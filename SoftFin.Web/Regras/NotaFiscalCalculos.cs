using SoftFin.NFSe.DTO;
using SoftFin.Utils;
using SoftFin.Web.Classes;
using SoftFin.Web.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Web;

namespace SoftFin.Web.Regras
{
    public class NotaFiscalCalculos
    {  

        public void ObtemCertificadoX509(Estabelecimento objEstab, out string caminhoArquivo, out System.Security.Cryptography.X509Certificates.X509Certificate2 cert)
        {
            var uploadPath = System.Web.Hosting.HostingEnvironment.MapPath("~/CertTMP/");
            Directory.CreateDirectory(uploadPath);

            var nomearquivonovo = Guid.NewGuid().ToString();

            caminhoArquivo = Path.Combine(@uploadPath, nomearquivonovo);

            cert = UtilSoftFin.BuscaCert(objEstab.id, objEstab.senhaCertificado, caminhoArquivo, objEstab.CNPJ);
        }

        public void AtualizaNFBussines(int id, ParamBase pb, 
                                DTONotaFiscal dTONF,
                                DbControle db, 
                                out NotaFiscal objNF, 
                                out DTORetornoNFEs resultado)
        {
            //var service = new SoftFin.NFSe.SaoPaulo.Bussiness.ConsultaNFe();
            var obj = new DTORetornoNFEs();
            
            //AtualizaNFServiceValidar(objEstab, objNF);
            var objEstab = pb.estab_id;
            objNF = new NotaFiscal().ObterPorId(id, db);
            var estabelecimento = new Estabelecimento().ObterPorId(pb.estab_id, pb);


            if (!string.IsNullOrEmpty(estabelecimento.MigrateCode))
            {
                var envio = new Migrate.NFSe.DTO.Consulta();
                envio = new ConversaoMigrateServico().ConverterDTOMigrateConsulta(estabelecimento, objNF);
                var service = new SoftFin.Migrate.NFSe.Business.ConsultaNFServico();
                var codeEmpresa = ConfigurationManager.AppSettings["MigrateCode"].ToString();
                var resultadoMigrate = service.Execute(envio, codeEmpresa, estabelecimento.MigrateCode);
                resultado = ConversaoMigrateServico.RetornoMigrateToRetornoConsultaDto(resultadoMigrate);
                if (resultado.NFe.Count() > 0)
                    resultado.NFe.First().DataEmissaoNFe = objNF.dataEmissaoRps.ToString("o");
            }
            else
            {
                if (dTONF == null)
                {
                    dTONF = new DTONotaFiscal();
                    var listaNFs = new List<NotaFiscal>();
                    listaNFs.Add(objNF);
                    new Conversao().ConverterNFEs(dTONF, estabelecimento, listaNFs);
                }
                string caminhoArquivo = "";
                X509Certificate2 cert;
                new NotaFiscalCalculos().ObtemCertificadoX509(estabelecimento, out caminhoArquivo, out cert);

                var regra = new SoftFin.NFSe.Business.SFConsultaNFSe();

                var result = regra.Execute(dTONF, cert);
                resultado = result;
            }



        }

        public NotaFiscalJson Calcula(
                    string strCodigoServico,
                    DateTime data,
                    int idBanco,
                    int idOperacao,
                    decimal valor,
                    int unidadeNegocio,
                    int pessoa,
                    int idOrdemVenda,
                    Estabelecimento estab, 
                    NotaFiscal NotaFiscalCalculada,
                    ParamBase pb,
                    DbControle banco = null
            )
        {
            string erros = "";
            try
            {
                if (banco == null)
                    banco = new DbControle();

                var ov = new OrdemVenda();

                if (idOrdemVenda != 0)
                    ov = new OrdemVenda().ObterPorId(idOrdemVenda, banco);
                else
                {
                    ov.unidadeNegocio_ID = unidadeNegocio;
                    ov.pessoas_ID = pessoa;
                    ov.valor = valor;
                }
                //ov.statusParcela_ID = new StatusParcela().ObterTodos().Where(p => p.status == "Emitida").First().id;
                NotaFiscalCalculada.ordemVenda_id = ov.id;
                NotaFiscalCalculada = CalculaNota(NotaFiscalCalculada, ov, banco, strCodigoServico, idOperacao, data,estab, pb);

                NotaFiscalCalculada.valorNfse = valor;
                NotaFiscalCalculada.dataEmissaoNfse = data;
                NotaFiscalCalculada.codigoServico = strCodigoServico;
                NotaFiscalCalculada.banco_id = idBanco;
                NotaFiscalCalculada.operacao_id = idOperacao;


                var textoNota = MontaTextoNota(NotaFiscalCalculada, banco, ov, ref erros);


                NotaFiscalCalculada.discriminacaoServico = textoNota;

                var notaFiscal = new NotaFiscalJson();

                notaFiscal.aliquotaIrrf = NotaFiscalCalculada.aliquotaIrrf.ToString("n");
                notaFiscal.aliquotaISS = NotaFiscalCalculada.aliquotaISS.ToString("n");
                notaFiscal.bairroTomador = NotaFiscalCalculada.NotaFiscalPessoaTomador.bairro;
                notaFiscal.banco = NotaFiscalCalculada.banco; ;
                notaFiscal.banco_id = NotaFiscalCalculada.banco_id.Value;
                notaFiscal.basedeCalculo = NotaFiscalCalculada.basedeCalculo.ToString("n");
                notaFiscal.cepTomador = NotaFiscalCalculada.NotaFiscalPessoaTomador.cep;
                notaFiscal.cidadeTomador = NotaFiscalCalculada.NotaFiscalPessoaTomador.cidade;
                notaFiscal.cnpjCpf = NotaFiscalCalculada.NotaFiscalPessoaTomador.cnpjCpf;
                notaFiscal.codigoServico = NotaFiscalCalculada.codigoServico;
                notaFiscal.codigoVerificacao = NotaFiscalCalculada.codigoVerificacao;
                notaFiscal.cofinsRetida = NotaFiscalCalculada.cofinsRetida.ToString("n");
                notaFiscal.complementoTomador = NotaFiscalCalculada.NotaFiscalPessoaTomador.complemento;
                notaFiscal.creditoImposto = NotaFiscalCalculada.creditoImposto.ToString("n");
                notaFiscal.csllRetida = NotaFiscalCalculada.csllRetida.ToString("n"); ;
                notaFiscal.dataEmissaoNfse = NotaFiscalCalculada.dataEmissaoNfse.ToShortDateString();
                notaFiscal.dataEmissaoRps = NotaFiscalCalculada.dataEmissaoRps.ToShortDateString();
                notaFiscal.dataVencimentoNfse = NotaFiscalCalculada.dataVencimentoNfse.ToShortDateString();
                notaFiscal.discriminacaoServico = NotaFiscalCalculada.discriminacaoServico;
                notaFiscal.emailTomador = NotaFiscalCalculada.NotaFiscalPessoaTomador.email;
                notaFiscal.enderecoTomador = NotaFiscalCalculada.NotaFiscalPessoaTomador.endereco;
                notaFiscal.indicadorCnpjCpf = NotaFiscalCalculada.NotaFiscalPessoaTomador.indicadorCnpjCpf;
                notaFiscal.inscricaoEstadual = NotaFiscalCalculada.NotaFiscalPessoaTomador.inscricaoEstadual;
                notaFiscal.inscricaoMunicipal = NotaFiscalCalculada.NotaFiscalPessoaTomador.inscricaoMunicipal;
                notaFiscal.irrf = NotaFiscalCalculada.irrf.ToString("n");
                notaFiscal.numeroNfse = NotaFiscalCalculada.numeroNfse;
                notaFiscal.numeroRps = NotaFiscalCalculada.numeroRps;
                notaFiscal.numeroTomador = NotaFiscalCalculada.NotaFiscalPessoaTomador.numero;
                notaFiscal.Operacao = NotaFiscalCalculada.Operacao;
                notaFiscal.operacao_id = NotaFiscalCalculada.operacao_id.Value;
                notaFiscal.pisRetido = NotaFiscalCalculada.pisRetido.ToString("n");
                notaFiscal.razaoTomador = NotaFiscalCalculada.NotaFiscalPessoaTomador.razao;
                notaFiscal.Recebimentos = NotaFiscalCalculada.Recebimentos;
                notaFiscal.serieRps = NotaFiscalCalculada.serieRps;
                notaFiscal.situacaoPrefeitura_id = NotaFiscalCalculada.situacaoPrefeitura_id;
                notaFiscal.situacaoRps = NotaFiscalCalculada.situacaoRps;
                notaFiscal.tipoEndereco = NotaFiscalCalculada.NotaFiscalPessoaTomador.tipoEndereco;
                notaFiscal.ufTomador = NotaFiscalCalculada.NotaFiscalPessoaTomador.uf;
                notaFiscal.valorDeducoes = NotaFiscalCalculada.valorDeducoes.ToString("n");
                notaFiscal.valorISS = NotaFiscalCalculada.valorISS.ToString("n");
                if (NotaFiscalCalculada.valorINSS == null)
                {
                    NotaFiscalCalculada.valorINSS = 0;
                }
                notaFiscal.valorINSS = NotaFiscalCalculada.valorINSS.Value.ToString("n");
                    
                if (NotaFiscalCalculada.aliquotaINSS == null)
                {
                    NotaFiscalCalculada.aliquotaINSS = 0;
                }
                notaFiscal.aliquotaINSS = NotaFiscalCalculada.aliquotaINSS.Value.ToString("n");
                notaFiscal.valorLiquido = NotaFiscalCalculada.valorLiquido.ToString("n");
                notaFiscal.valorNfse = NotaFiscalCalculada.valorNfse.ToString("n");
                notaFiscal.ValAliqISSRetido = NotaFiscalCalculada.ValAliqISSRetido.ToString("n");
                notaFiscal.ValAliqCSLL = NotaFiscalCalculada.ValAliqCSLL.ToString("n");
                notaFiscal.ValAliqPIS = NotaFiscalCalculada.ValAliqPIS.ToString("n");
                notaFiscal.ValISSRetido = NotaFiscalCalculada.ValISSRetido.ToString("n");
                
                

                return notaFiscal;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public static NotaFiscal CalculaNota(
                NotaFiscal nota,
                OrdemVenda ov,
                DbControle banco,
                string codigoServico,
                int operacao_id,
                DateTime data,
                Estabelecimento estab,
                ParamBase pb)
        {
            try
            {


                Pessoa pessoa = banco.Pessoa.ToList().Where(z => z.id == ov.pessoas_ID).FirstOrDefault();
                //TipoEndereco tipoEndereco = banco.TipoEndereco.ToList().Where(t => t.id == pessoa.TipoEndereco.id).FirstOrDefault();
                int estaid = estab.id;


                //Seleciona Código de Serviço
                nota.dataVencimentoNfse = Convert.ToDateTime(data);

                //Dados da empresa
                nota.estabelecimento_id = estab.id;
                NotaFiscal rps = new NotaFiscal();
                rps = banco.NotaFiscal.Where(p => p.estabelecimento_id == nota.estabelecimento_id).ToList().OrderByDescending(r => r.numeroRps).FirstOrDefault();

                //Dados do RPS
                nota.tipoRps = 1;
                nota.serieRps = "U";
                if (rps == null)
                    nota.numeroRps = 1;
                else
                    nota.numeroRps = (rps.numeroRps + 1);
                nota.situacaoRps = "1";

                //Dados do tomador
                nota.NotaFiscalPessoaTomador = new NotaFiscalPessoa();
                nota.NotaFiscalPessoaTomador.razao = pessoa.razao;
                if (pessoa.CategoriaPessoa_ID != null)
                    nota.NotaFiscalPessoaTomador.indicadorCnpjCpf = pessoa.CategoriaPessoa_ID.Value;

                nota.NotaFiscalPessoaTomador.cnpjCpf = pessoa.cnpj;
                nota.NotaFiscalPessoaTomador.inscricaoMunicipal = pessoa.ccm;
                nota.NotaFiscalPessoaTomador.inscricaoEstadual = pessoa.inscricao;
                if (pessoa.TipoEndereco != null)
                    nota.NotaFiscalPessoaTomador.tipoEndereco = pessoa.TipoEndereco.Descricao;
                else
                    nota.NotaFiscalPessoaTomador.tipoEndereco = "";

                nota.NotaFiscalPessoaTomador.endereco = pessoa.endereco;
                nota.NotaFiscalPessoaTomador.numero = pessoa.numero;
                nota.NotaFiscalPessoaTomador.complemento = pessoa.complemento;
                nota.NotaFiscalPessoaTomador.bairro = pessoa.bairro;
                nota.NotaFiscalPessoaTomador.cidade = pessoa.cidade;
                nota.NotaFiscalPessoaTomador.uf = pessoa.uf;
                nota.NotaFiscalPessoaTomador.cep = pessoa.cep;
                nota.NotaFiscalPessoaTomador.endereco = pessoa.endereco;
                nota.NotaFiscalPessoaTomador.email = pessoa.eMail;

                //Detalhes de Nota
                nota.valorNfse = ov.valor;


                var aliquotas = new calculoImposto().ObterTodos(pb, banco).Where(p => p.operacao_id == operacao_id);

                //Cálculo do IRRF

                nota.basedeCalculo = ov.valor - nota.valorDeducoes;

                nota.irrf = 0;
                //nota.valorDeducoes = 0;
                nota.aliquotaIrrf = 0;

                //Carrega variável quando flag retido estiver marcado
                var impostoAux = CalculaImposto(aliquotas, "IRRF");

                if (impostoAux != null)
                {
                    if (impostoAux.retido)
                    {
                        nota.irrf = nota.basedeCalculo * impostoAux.aliquota / 100;
                        //nota.valorDeducoes = nota.valorDeducoes + nota.irrf;
                        nota.aliquotaIrrf = impostoAux.aliquota;
                    }
                }


                //Cálculo do ISS
                impostoAux = CalculaImposto(aliquotas, "ISS");

                nota.aliquotaISS = 0;
                nota.valorISS = 0;

                if (impostoAux != null)
                {
                    if (impostoAux.retido)
                    {
                        nota.valorISS = nota.basedeCalculo * impostoAux.aliquota / 100;
                        //nota.valorDeducoes = nota.valorDeducoes + nota.valorISS;
                        nota.aliquotaISS = impostoAux.aliquota;
                    }
                }

                //Cálculo do ISSRET
                impostoAux = CalculaImposto(aliquotas, "ISSRET");

                nota.ValISSRetido = 0;
                nota.ValAliqISSRetido = 0;

                if (impostoAux != null)
                {
                    if (impostoAux.retido)
                    {
                        nota.ValISSRetido = nota.basedeCalculo * impostoAux.aliquota / 100;
                        //nota.valorDeducoes = nota.valorDeducoes + nota.ValISSRetido;
                        nota.ValAliqISSRetido = impostoAux.aliquota;
                        
                    }
                }


                //Cálculo do INSS
                impostoAux = CalculaImposto(aliquotas, "INSS");

                nota.valorINSS = 0;
                nota.aliquotaINSS = 0;

                if (impostoAux != null)
                {
                    if (impostoAux.retido)
                    {
                        nota.valorINSS = nota.basedeCalculo * impostoAux.aliquota / 100;
                        //nota.valorDeducoes = nota.valorDeducoes + nota.valorINSS.Value;
                        nota.aliquotaINSS = impostoAux.aliquota;
                    }
                }


                //Cálculo do PIS
                impostoAux = CalculaImposto(aliquotas, "PIS");

                nota.pisRetido = 0;
                nota.ValAliqPIS = 0;

                if (impostoAux != null)
                {
                    if (impostoAux.retido)
                    {
                        nota.pisRetido = nota.basedeCalculo * impostoAux.aliquota / 100;
                        //nota.valorDeducoes = nota.valorDeducoes + nota.pisRetido;
                        nota.ValAliqPIS = impostoAux.aliquota;
                    }
                }

                //Cálculo da COFINS
                impostoAux = CalculaImposto(aliquotas, "COFINS");

                nota.cofinsRetida = 0;
                nota.ValAliqCOFINS = 0;

                if (impostoAux != null)
                {
                    if (impostoAux.retido)
                    {
                        nota.cofinsRetida = nota.basedeCalculo * impostoAux.aliquota / 100;
                        //nota.valorDeducoes = nota.valorDeducoes + nota.cofinsRetida;
                        nota.ValAliqCOFINS = impostoAux.aliquota;
                    }
                }


                //Cálculo da CSLL
                impostoAux = CalculaImposto(aliquotas, "CSLL");

                nota.csllRetida = 0;
                nota.ValAliqCSLL = 0;

                if (impostoAux != null)
                {
                    if (impostoAux.retido)
                    {
                        nota.csllRetida = nota.basedeCalculo * impostoAux.aliquota / 100;
                        //nota.valorDeducoes = nota.valorDeducoes + nota.csllRetida;
                        nota.ValAliqCSLL = impostoAux.aliquota;
                    }
                }

                nota.creditoImposto = 0;

                //Dados do Contrato
                nota.codigoServico = codigoServico;
                //nota.valorDeducoes = 0;
                //Atualiza valor líquido da nota com base nas retenções
                decimal impostosRetidos = 
                    nota.csllRetida + 
                    nota.cofinsRetida +
                    nota.pisRetido +
                    nota.valorINSS.Value +
                    nota.ValISSRetido +
                    nota.irrf
                    ;

                nota.valorLiquido = nota.valorNfse - impostosRetidos;
                nota.municipio_id = estab.Municipio_id;
                nota.operacao_id = operacao_id;
                nota.ordemVenda_id = ov.id;
                nota.dataEmissaoRps = SoftFin.Utils.UtilSoftFin.DateTimeBrasilia(); //DateTime.Now;
                return nota;
            }
            catch 
            {
                return nota;
            }

        }

        private static calculoImposto CalculaImposto(IEnumerable<calculoImposto> aliquotas, string imposto)
        {
            var aliquota = aliquotas.Where(p => p.imposto.codigo.Equals(imposto)).FirstOrDefault();
            return aliquota;
        }

        public string MontaTextoNota(NotaFiscal nota, DbControle banco, OrdemVenda ov, ref string Erros)
        {
            string retorno = "";
            string textoNota = "";
            string Campo = "";
            string Model = "";

            try
            {
                List<parametroDescricaoNota> parametros = banco.parametroDescricaoNota.ToList();
                Banco bancoAux = banco.Bancos.Where(p => p.id == nota.banco_id).FirstOrDefault();

                textoNota = banco.Operacao.Where(p => p.id == nota.operacao_id).First().descricaoNota;
                parametros = banco.parametroDescricaoNota.ToList();
                bancoAux = banco.Bancos.Where(p => p.id == nota.banco_id).FirstOrDefault();


                if (!string.IsNullOrEmpty(textoNota))
                {
                    foreach (var item in parametros)
                    {
                        Campo = item.campo;
                        Model = item.nomemodel;

                        if (textoNota.ToUpper().IndexOf(item.hashtag.ToUpper()) > -1)
                        {
                            if (item.nomemodel.ToUpper() == "CONTRATO")
                            {
                                try
                                {
                                    var Property = ov.ParcelaContrato.ContratoItem.Contrato.GetType().GetProperty(item.campo);
                                    retorno = Property.GetValue(ov.ParcelaContrato.ContratoItem.Contrato, null) as string;
                                }
                                catch
                                {
                                }
                            }
                            else if (item.nomemodel.ToUpper() == "NOTAFISCAL")
                            {
                                try
                                {
                                    var Property = nota.GetType().GetProperty(item.campo);
                                    retorno = FormataNota(nota, retorno, Property);
                                }
                                catch
                                {
                                }
                            }
                            else if (item.nomemodel.ToUpper() == "BANCO")
                            {
                                try
                                {
                                    var Property = bancoAux.GetType().GetProperty(item.campo);
                                    retorno = FormataNota(bancoAux, retorno, Property);
                                }
                                catch
                                {
                                }
                            }
                            else if (item.nomemodel.ToUpper() == "CONTRATOITEM")
                            {
                                try
                                {
                                    var Property = ov.ParcelaContrato.ContratoItem.GetType().GetProperty(item.campo);
                                    retorno = FormataNota(ov.ParcelaContrato.ContratoItem, retorno, Property);
                                }
                                catch
                                {
                                }
                            }
                            else if (item.nomemodel.ToUpper() == "ORDEMVENDA")
                            {
                                try
                                {
                                    var Property = ov.GetType().GetProperty(item.campo);
                                    retorno = FormataNota(ov, retorno, Property);
                                }
                                catch
                                {
                                }
                            }

                            retorno = retorno.Replace("00:00:00", "");
                            textoNota = textoNota.Replace(item.hashtag, retorno);
                        }
                    }
                }
            }
            catch 
            {
                Erros = string.Format("Model {1} campo {0} erro na extração de dados. Erro Original : ", Campo, Model);
                throw new Exception(Erros); ;
            }

            return textoNota;
        }

        private static string FormataNota(object nota, string retorno, PropertyInfo Property)
        {
            if (Property.PropertyType.Name == "Decimal")
            {
                retorno = Property.GetValue(nota, null).ToString();
                retorno = Decimal.Parse(retorno).ToString("0.00");
            }
            else
                retorno = Property.GetValue(nota, null).ToString();
            return retorno;
        }


        public void BaixaNF(NotaFiscal nf, 
            DateTime dataEmissao, 
            string codigoVerificacao, 
            int numeroNFE, 
            string tipo, 
            DbControle db, 
            ParamBase pb)
        {
            if (nf != null)
            {
                nf.dataEmissaoNfse = dataEmissao;
                nf.codigoVerificacao = codigoVerificacao;
                nf.numeroNfse = numeroNFE;
                if (tipo == null)
                {
                    tipo = "";
                }
                if (tipo.Equals("C"))
                {
                    nf.situacaoPrefeitura_id = Models.NotaFiscal.NFCANCELADACCONF;
                }
                else
                {
                    nf.situacaoPrefeitura_id = Models.NotaFiscal.NFGERADAENVIADA;

                    int? idNf = nf.id;
                    BancoMovimento bancoMovimento = new BancoMovimento().ObterTodos(pb).Where(p => p.notafiscal_id == idNf).FirstOrDefault();

                    if (bancoMovimento == null)
                    {

                        BancoMovimento bancoMovimentoNew = new BancoMovimento();
                        bancoMovimentoNew.banco_id = nf.banco_id.Value;
                        bancoMovimentoNew.data = nf.dataVencimentoNfse;

                        bancoMovimentoNew.valor = nf.valorNfse;
                        bancoMovimentoNew.planoDeConta_id = 4;
                        bancoMovimentoNew.tipoDeMovimento_id = new TipoMovimento().TipoEntrada(pb);
                        bancoMovimentoNew.tipoDeDocumento_id = new TipoDocumento().TipoNota();
                        //Grava no banco movimento Origem="Faturamento"
                        bancoMovimentoNew.origemmovimento_id = new OrigemMovimento().TipoFaturamento(pb);
                        bancoMovimentoNew.planoDeConta_id = new PlanoDeConta().ObterPorCodigo("01.01", db).id;
                        bancoMovimentoNew.historico = "NFSe Nº " + nf.numeroNfse.ToString();
                        bancoMovimentoNew.notafiscal_id = nf.id;
                        //Inclui novo banco movimento
                        bancoMovimentoNew.Incluir(bancoMovimentoNew, pb);
                    }
                }
                
                db.SaveChanges();
            }
        }

    }
}