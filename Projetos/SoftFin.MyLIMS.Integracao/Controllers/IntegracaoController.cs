using Newtonsoft.Json;
using SoftFin.API.DTO;
using SoftFin.MyLIMS.Integracao.CallApi;
using SoftFin.MyLIMS.Integracao.DTO;
using SoftFin.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace SoftFin.MyLIMS.Integracao.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class IntegracaoController : ApiController
    {
        [Route("Integracao/OK")]
        [HttpGet]
        public string OK()
        {
            return "OK";
        }


        [Route("Integracao/Diaria")]
        [HttpGet]
        public void Integracao()
        {
            var processoIntegrado = Guid.NewGuid();
            var DTOContratos = new List<API.DTO.DTOContrato>();
            
            
            DTOContratos = ConvertMyLimsToSoftFin(processoIntegrado);

            try
            {
                ImportaSoftFin(DTOContratos, processoIntegrado);
            }
            finally
            {
                GeraEmailResultado(processoIntegrado, DTOContratos);
            }
        }


        private string GetClientIp(HttpRequestMessage request = null)
        {
            request = request ?? Request;

            if (request.Properties.ContainsKey("MS_HttpContext"))
            {
                return ((HttpContextWrapper)request.Properties["MS_HttpContext"]).Request.UserHostAddress;
            }
            //else if (request.Properties.ContainsKey(RemoteEndpointMessageProperty.Name))
            //{
            //    RemoteEndpointMessageProperty prop = (RemoteEndpointMessageProperty)request.Properties[RemoteEndpointMessageProperty.Name];
            //    return prop.Address;
            //}
            else if (HttpContext.Current != null)
            {
                return HttpContext.Current.Request.UserHostAddress;
            }
            else
            {
                return null;
            }
        }

        [Route("Integracao/PorContrato")]
        [HttpGet]
        public void PorContrato(string contrato)
        {
            var processoIntegrado = Guid.NewGuid();
            var DTOContratos = new List<API.DTO.DTOContrato>();

            new Log().PostImportar(new Infrastructure.DTO.DTOLogImportar
            {
                Agrupador = processoIntegrado,
                Data = SoftFin.Utils.UtilSoftFin.DateTimeBrasilia(),
                Tipo = "Inicio",
                Descricao = "Inicio Integração por Contrato ",
                Ip = GetClientIp(),
                Usuario = "Integrador",
                Registro = contrato
            });
            contrato = contrato.Replace("?contrato=", "");
            DTOContratos = ConvertMyLimsToSoftFin(processoIntegrado).Where(p => p.Codigo == contrato).ToList();

            try
            {
                ImportaSoftFin(DTOContratos, processoIntegrado);
            }
            catch(Exception ex)
            {
                new Log().PostImportar(new Infrastructure.DTO.DTOLogImportar
                {
                    Agrupador = processoIntegrado,
                    Tipo = "Erro",
                    Descricao = "Fim Importação ",
                    Ip = GetClientIp(),
                    Usuario = "Integrador",
                    Json = ex.ToString()
                });
            }
            finally
            {
                new Log().PostImportar(new Infrastructure.DTO.DTOLogImportar
                {
                    Agrupador = processoIntegrado,
                    Tipo = "Fim",
                    Descricao = "Fim Importação ",
                    Ip = GetClientIp(),
                    Usuario = "Integrador"
                });
                GeraEmailResultado(processoIntegrado, DTOContratos);
            }
        }


        [Route("Integracao/Pendentes")]
        [HttpGet]
        public List<DTOContrato> Pendentes()
        {
            var processoIntegrado = Guid.NewGuid();

            new Log().PostImportar(new Infrastructure.DTO.DTOLogImportar
            {
                Agrupador = processoIntegrado,
                Tipo = "Inicio",
                Descricao = "Consulta Importação",
                Ip = GetClientIp(),
                Usuario = "Integrador"
            });

           
            var DTOContratos = new List<API.DTO.DTOContrato>();
            DTOContratos = ConvertMyLimsToSoftFin(processoIntegrado);
            return DTOContratos;
        }


        private void GeraEmailResultado(Guid processoIntegrado, List<API.DTO.DTOContrato> lista)
        {
            StringBuilder sb = new StringBuilder(); 
            foreach (var item in lista.OrderBy(p => p.Resultado))
            {
                sb.AppendLine("Contrato " + item.Codigo + " = " + item.Resultado + "<br />");
            }

            EnviaEmail("Resultado da Integração executado em " + SoftFin.Utils.UtilSoftFin.DateTimeBrasilia(),
                sb.ToString(),
                ConfigurationManager.AppSettings["email"].ToString());
        }
        private void EnviaEmail(string titulo, string corpo, string emailaviso)
        {
            var email = new Email();
            var path = System.Web.Hosting.HostingEnvironment.MapPath("~/EmailTemplate/");
            var arquivohmtl = Path.Combine(path, "Emailresultadointegracao.html");
            string readText = File.ReadAllText(arquivohmtl);
            readText = readText.Replace("{Titulo}", titulo);
            readText = readText.Replace("{Corpo}", corpo);
            readText = readText.Replace("{nomeestab}","ConforLab");
            email.EnviarMensagem(emailaviso, titulo, readText, true);
        }

        private void ImportaSoftFin(
                List<DTOContrato> dTOContratos,
                Guid processoIntegrado
            )
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();

            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
            settings.CheckAdditionalContent = false;
            settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

            foreach (var item in dTOContratos)
            {
                foreach (var itemCliente in item.DTOClientes)
                {

                    new Log().PostImportar(new Infrastructure.DTO.DTOLogImportar
                    {
                        Agrupador = processoIntegrado,
                        Tipo = "Alerta",
                        Descricao = "Importando Cliente",
                        Ip = GetClientIp(Request),
                        Usuario = "Integrador",
                        Registro = item.Codigo,
                        Json = JsonConvert.SerializeObject(itemCliente, settings)
                    });

                    var pessoa = new CallApi.Destino().PostPessoa(itemCliente);
                }
                foreach (var itemVendedor in item.DTOVendedor)
                {
                    new Log().PostImportar(new Infrastructure.DTO.DTOLogImportar
                    {
                        Agrupador = processoIntegrado,
                        Tipo = "Alerta",
                        Descricao = "Importando Vendedor",
                        Ip = GetClientIp(),
                        Usuario = "Integrador",
                        Registro = item.Codigo,
                        Json = JsonConvert.SerializeObject(itemVendedor, settings)
                    });
                    var pessoa = new CallApi.Destino().PostPessoa(itemVendedor);
                }

                if (new CallApi.Destino().GetContratoPorCodigo(item.Codigo) == false)
                {
                    if (item.Resultado.Contains("(99)"))
                    {
                        if (item.Valor == 0)
                        {
                            new Log().PostImportar(new Infrastructure.DTO.DTOLogImportar
                            {
                                Agrupador = processoIntegrado,
                                Tipo = "Alerta",
                                Descricao = "Contrato não pode ser importado valor Zero",
                                Ip = GetClientIp(),
                                Usuario = "Integrador",
                                Registro = item.Codigo,
                                Json = JsonConvert.SerializeObject(item, settings)
                            });

                            item.Resultado = "(10) Contrato Não Importado, Valor Zero;";
                        }
                        else
                        {
                            var contrato = new CallApi.Destino().PostContrato(item);
                            if (contrato.status == "OK")
                            {
                                //new CallApi.Origem().PutMarcaReferenceKey(item.CodigoOrigem, item.Codigo);

                                new Log().PostImportar(new Infrastructure.DTO.DTOLogImportar
                                {
                                    Agrupador = processoIntegrado,
                                    Tipo = "Alerta",
                                    Descricao = "Importando Contrato",
                                    Ip = GetClientIp(),
                                    Usuario = "Integrador",
                                    Registro = item.Codigo,
                                    Json = JsonConvert.SerializeObject(item, settings)
                                });
                                item.Resultado = "(00) Contrato Importado;";
                            }
                            else
                            {


                                item.Resultado = "(11) Contrato Não Importado;";

                                foreach (var itemExp in contrato.exceptions)
                                {
                                    item.Resultado += "(" + itemExp.codigo + itemExp.descricao + ")";
                                }

                                new Log().PostImportar(new Infrastructure.DTO.DTOLogImportar
                                {
                                    Agrupador = processoIntegrado,
                                    Tipo = "Alerta",
                                    Descricao = "Contrato não Importado",
                                    Ip = GetClientIp(),
                                    Usuario = "Integrador",
                                    Registro = item.Codigo,
                                    Mensagem = item.Resultado,
                                    Json = JsonConvert.SerializeObject(item, settings)
                                });
                            }
                        }
                    }
                    else
                    {
                        new Log().PostImportar(new Infrastructure.DTO.DTOLogImportar
                        {
                            Agrupador = processoIntegrado,
                            Tipo = "Alerta",
                            Descricao = "Contrato não Importado",
                            Ip = GetClientIp(),
                            Usuario = "Integrador",
                            Registro = item.Codigo,
                            Mensagem = item.Resultado,
                            Json = JsonConvert.SerializeObject(item, settings)
                        });
                    }
                }
                else
                {
                    new Log().PostImportar(new Infrastructure.DTO.DTOLogImportar
                    {
                        Agrupador = processoIntegrado,
                        Tipo = "Alerta",
                        Descricao = "Contrato Já Importato",
                        Ip = GetClientIp(),
                        Usuario = "Integrador",
                        Registro = item.Codigo,
                        Json = JsonConvert.SerializeObject(item, settings)
                    });

                    item.Resultado = "(01) Contrato já importado no SoftFin";
                }
            }
        }

        private List<DTOContrato> ConvertMyLimsToSoftFin(Guid processoIntegrado)
        {
            //Versão Final
            //var wks2 = new CallApi.Origem().GetWorks();

            //Versão Teste
            //var wks = new CallApi.Origem().GetWorksRegra();

            //Versao 30
            var wks = new Works.RootObject();
            List<string> contratos = new List<string>();



            contratos.Add("PC87/2018");
            contratos.Add("PC123/2018");
            contratos.Add("PC218/2018");
            contratos.Add("PC274/2018");
            contratos.Add("PC307/2018");
            contratos.Add("PC328/2018");
            contratos.Add("PC423/2018");
            contratos.Add("PC192/2018");
            contratos.Add("PC242/2018");
            contratos.Add("PC270/2018");
            contratos.Add("PC589/2018");



            wks.Result = new List<Works.Result>();
            foreach (var item in contratos)
            {
                var wksAux = new CallApi.Origem().GetWorksfilterControlNumber(item);
                if (wksAux.Result.FirstOrDefault() != null)
                {
                    var aux = wksAux.Result.FirstOrDefault();
                    if (aux != null)
                        wks.Result.Add(aux);
                }
            }

            var retorno = new List<DTOContrato>();

            foreach (var item in wks.Result)
            {

                var dtoContrato = new DTOContrato();
                dtoContrato.Resultado = "(99) Não Executado...";
                var wksinfo = new CallApi.Origem().WorksInfo(item.Id.ToString());
                var wkssamples = new CallApi.Origem().WorksSamples(item.Id.ToString());
                var wksprices = new CallApi.Origem().WorksPriceItens(item.Id.ToString());
                var unidadesnegocios = new CallApi.Destino().GetUnidadeNegocio();
                var deParasMyLimsAmostras = new CallApi.Destino().GetDeParaMyLimsAmostras();
                var deParasMyLimsPrecos = new CallApi.Destino().GetDeParaMyLimsPrecos();
                var accounts = new CallApi.Origem().GetAccounts(item.Id.ToString());

                dtoContrato.ProcessoIntegrado = processoIntegrado;
                dtoContrato.CodigoOrigem = item.Id.ToString();
                dtoContrato.Codigo = item.ControlNumber;
                dtoContrato.CodigoCliente = item.Account.Complement;
                dtoContrato.DataEmissao = item.CurrentWorkFlow.Execution;
                dtoContrato.InicioVigencia = item.CurrentWorkFlow.Execution;
                dtoContrato.FimVigencia = item.CurrentWorkFlow.Conclusion;
                dtoContrato.Descricao = item.Identification;

                var listaUnidadeSamples = new List<string>();

                foreach (var itemSum in wkssamples.Result)
                {
                    //if (itemSum.Sample.CurrentStatus.SampleStatus.Id != 8)
                    //    wkssamples.Result.Remove(itemSum);
                    //else
                    //{
                    var samplesSummaryPrices = new CallApi.Origem().SamplesSummaryPrices(itemSum.Sample.Id.ToString());
                    itemSum.valor = samplesSummaryPrices;
                    var strunidade = itemSum.Sample.Identification.Trim();
                    var depara = deParasMyLimsAmostras.objs.Where(p => p.De == strunidade).FirstOrDefault();
                    if (depara != null)
                    {
                        if (!listaUnidadeSamples.Contains(depara.Para.Trim()))
                            listaUnidadeSamples.Add(depara.Para.Trim());
                    }

                }


                var clsample = wkssamples.Result.Where(p => p.Sample.CollectionPoint != null).FirstOrDefault();
                //TODO MElhorar
                if (clsample != null)
                {
                    var collectionPoint = new CallApi.Origem().GetCollectionPoint(clsample.Sample.CollectionPoint.Id.ToString());
                    dtoContrato.CodigoMunicipioIBGE = collectionPoint.City.Identification;
                }
                else
                {
                    var collectionPoint = new CallApi.Origem().GetAccountAddresses(item.Account.Id.ToString());

                    if (collectionPoint.Result.Where(p => p.AddressType.Identification == "Faturamento").Count() == 1)
                    {
                        var endereco = collectionPoint.Result.Where(p => p.AddressType.Identification == "Faturamento").First();
                        dtoContrato.CodigoMunicipioIBGE = endereco.City.Identification;

                        var pessoa =
                        new DTOPessoa
                        {
                            CNPJ = dtoContrato.CodigoCliente,
                            Nome = item.Account.Identification,
                            Razao = item.Account.Identification,
                            Codigo = dtoContrato.CodigoCliente,
                            CodigoEstab = "001",
                            TipoPessoa = "Cliente",
                            Logradouro = endereco.Address1,
                            Cidade = endereco.City.Identification,
                            Bairro = endereco.District,
                            Cep = endereco.ZipCode
                        };

                        pessoa.DTOContatos = new List<DTOContato>();
                        if (accounts.Count != 0)
                        {
                            foreach (var itemX in accounts.Result)
                            {
                                var dTOContato = new DTOContato();
                                dTOContato.Nome = itemX.Account.Identification;

                                if (itemX.Account.AccountType.Identification.Equals("Faturamento"))
                                    dTOContato.RecebeCobranca = true;

                                pessoa.DTOContatos.Add(dTOContato);
                            }
                        }

                        dtoContrato.DTOClientes.Add(pessoa);
                    }
                    else if (collectionPoint.Result.Where(p => p.AddressType.Identification == "Principal").Count() == 1)
                    {
                        var endereco = collectionPoint.Result.Where(p => p.AddressType.Identification == "Principal").First();
                        dtoContrato.CodigoMunicipioIBGE = endereco.City.Identification;

                        var pessoa =
                        new DTOPessoa
                        {
                            CNPJ = dtoContrato.CodigoCliente,
                            Nome = item.Account.Identification,
                            Razao = item.Account.Identification,
                            Codigo = dtoContrato.CodigoCliente,
                            CodigoEstab = "001",
                            TipoPessoa = "Cliente",
                            Logradouro = endereco.Address1,
                            Cidade = endereco.City.Identification,
                            Bairro = endereco.District,
                            Cep = endereco.ZipCode
                        };

                        pessoa.DTOContatos = new List<DTOContato>();
                        if (accounts.Count != 0)
                        {
                            foreach (var itemX in accounts.Result)
                            {
                                var dTOContato = new DTOContato();
                                dTOContato.Nome = itemX.Account.Identification;

                                pessoa.DTOContatos.Add(dTOContato);
                            }
                        }

                        dtoContrato.DTOClientes.Add(pessoa);

                    }
                    else
                    {
                        dtoContrato.Resultado = "(40) Municipio de coleta não encontrado";

                    }
                }


                var somaprecos = decimal.Parse(wksprices.Result.Sum(p => p.Price).ToString("0.00"));
                var valortotal = somaprecos + decimal.Parse(wkssamples.Result.Where(p => p.Sample.CurrentStatus.SampleStatus.Id == 8).Sum(p => p.valor.TotalPrice).ToString("0.00"));

                dtoContrato.ProcessoIntegrado = processoIntegrado;


                var listaUnidade = new List<string>();

                foreach (var itemUnidade in wksprices.Result)
                {
                    if (!listaUnidade.Contains(itemUnidade.PriceItem.Identification.Trim()))
                        listaUnidade.Add(itemUnidade.PriceItem.Identification.Trim());
                }

                var listaUnidadeDePara = new List<string>();

                foreach (var itemUnidade in listaUnidade)
                {
                    var depara = deParasMyLimsPrecos.objs.Where(p => p.De == itemUnidade).FirstOrDefault();
                    if (depara != null)
                    {
                        if (!listaUnidadeDePara.Contains(depara.Para.Trim()))
                            listaUnidadeDePara.Add(depara.Para.Trim());
                    }
                }




                var diaVencimento = 0;
                var banco = "";
                var prazo = "";
                var nomeVendedor = "";
                var instNF = "";

                if (wksinfo.Count != 0)
                {
                    foreach (var iteminfo in wksinfo.Result)
                    {
                        if (iteminfo.Info.Identification == "Condição de Pagamento")
                        {
                            dtoContrato.CondicaoPagamento = iteminfo.ValueText;
                        }
                        else if (iteminfo.Info.Identification == "Vendedor")
                        {
                            nomeVendedor = iteminfo.ValueText;
                            if (iteminfo.ValueText.Length > 20)
                                dtoContrato.CodigoVendedor = iteminfo.ValueText.Substring(0, 20);
                            else
                                dtoContrato.CodigoVendedor = iteminfo.ValueText;
                        }
                        else if (iteminfo.Info.Identification == "Dia de Vencimento")
                        {
                            int.TryParse(iteminfo.ValueText, out diaVencimento);
                        }
                        else if (iteminfo.Info.Identification == "Banco")
                        {
                            banco = iteminfo.ValueText;
                        }
                        else if (iteminfo.Info.Identification == "Prazo de Pagamento")
                        {
                            dtoContrato.Prazo = iteminfo.ValueText;
                        }
                        else if (iteminfo.Info.Identification == "Instruções Corpo da NF")
                        {
                            instNF = iteminfo.ValueText;
                        }

                    }
                }

                //dtoContrato.Resultado = "(99) Não Executado...";
                if (dtoContrato.CondicaoPagamento == null)
                {
                    dtoContrato.Valor = valortotal;
                }
                else if (dtoContrato.CondicaoPagamento.Contains("x"))
                {
                    dtoContrato.Valor = valortotal;
                    var parcela = int.Parse(dtoContrato.CondicaoPagamento.Replace("x", "").ToString());

                }



                dtoContrato.DTOVendedor.Add(new DTOPessoa
                {
                    Nome = nomeVendedor,
                    Razao = nomeVendedor,
                    Codigo = dtoContrato.CodigoVendedor,
                    CodigoEstab = "001",
                    TipoPessoa = "Vendedor"
                });

                var contratoItem = new DTOContratoItem();
                contratoItem.DTOContratoItemUnidades = new List<DTOContratoItemUnidade>();

                //TODO Ricardo
                //Soma precos e distribui entre as areas
                if (somaprecos != 0)
                {
                    decimal somaUnidadeForaDepara = 0;

                    foreach (var itemPrices in wksprices.Result)
                    {
                        var depara = deParasMyLimsPrecos.objs.Where(p => p.De == itemPrices.PriceItem.Identification).FirstOrDefault();
                        if (depara == null)
                        {
                            somaUnidadeForaDepara += decimal.Parse(itemPrices.Price.ToString("0.00"));

                        }
                        else
                        {
                            var unidade = new DTOContratoItemUnidade();
                            unidade.Descricao = "Prc." + itemPrices.PriceItem.Identification;
                            unidade.Unidade = depara.Para;
                            unidade.Valor = Decimal.Parse(itemPrices.Price.ToString("0.00"));
                            contratoItem.DTOContratoItemUnidades.Add(unidade);
                        }
                    }

                    if (listaUnidadeDePara.Count != 0)
                    {
                        foreach (var itemunid in listaUnidadeDePara)
                        {
                            var unidade = new DTOContratoItemUnidade();
                            unidade.Descricao = "Prc." + itemunid + "-" + somaUnidadeForaDepara.ToString();
                            unidade.Unidade = itemunid;
                            unidade.Valor = decimal.Round(somaUnidadeForaDepara / listaUnidadeDePara.Count(), 2);
                            contratoItem.DTOContratoItemUnidades.Add(unidade);

                        }
                    }
                    else
                    {
                        foreach (var itemunid in listaUnidadeSamples)
                        {
                            var unidade = new DTOContratoItemUnidade();
                            unidade.Descricao = "AmoDiv." + itemunid + "-" + somaUnidadeForaDepara.ToString();
                            unidade.Unidade = itemunid;
                            unidade.Valor = decimal.Round(somaUnidadeForaDepara / listaUnidadeSamples.Count(), 2);
                            contratoItem.DTOContratoItemUnidades.Add(unidade);

                        }

                    }

                }



                //Executa Depara de Areas

                foreach (var itemamostra in wkssamples.Result)
                {
                    var depara = deParasMyLimsAmostras.objs.Where(p => p.De == itemamostra.Sample.SampleType.Identification).FirstOrDefault();
                    if (depara == null)
                    {
                        dtoContrato.Resultado = "(31) De para não encontrado - " + depara.De;
                        break;
                    }
                    else
                    {
                        var unidade = new DTOContratoItemUnidade();

                        unidade.Descricao = "Amo." + itemamostra.Sample.Identification;
                        unidade.Unidade = depara.Para;
                        unidade.Valor = Decimal.Parse(itemamostra.valor.TotalPrice.ToString("0.00"));
                        contratoItem.DTOContratoItemUnidades.Add(unidade);
                    }
                }

                contratoItem.Codigo = dtoContrato.Codigo;
                //Pedido
                //Contrato Normal
                //Contrato por medição
                contratoItem.TipoContrato = item.WorkClass.Identification;
                contratoItem.Valor = dtoContrato.Valor;

                if (dtoContrato.FimVigencia >= DateTime.Now) // Se a data de vigencia inferior a atual nao gera parcela
                {

                    dtoContrato.DTOContratoItems = new List<DTOContratoItem>();
                    var parcela = new DTOParcelaContrato();

                    if (dtoContrato.CondicaoPagamento.Contains("x"))
                    {
                        parcela.Codigo = instNF;
                        parcela.Recorrente = ENUMRecorrente.SimReverso;
                        parcela.Descricao = "Pagamento em " + dtoContrato.Prazo;
                        parcela.Parcela = int.Parse(dtoContrato.CondicaoPagamento.Replace("x", "").ToString());
                        parcela.Data = dtoContrato.FimVigencia.Value;
                        parcela.DiaVencimento = diaVencimento;
                        parcela.Prazo = dtoContrato.Prazo;
                        parcela.Valor = contratoItem.Valor;
                        parcela.BancoReferencia = banco;
                    }
                    else {
                        parcela.Codigo = instNF;
                        parcela.Recorrente = ENUMRecorrente.Não;
                        parcela.Descricao = "Pagamento em " + dtoContrato.Prazo;
                        parcela.Parcela = 1;
                        parcela.Data = dtoContrato.DataEmissao.Value;
                        parcela.DiaVencimento = diaVencimento;
                        parcela.Prazo = dtoContrato.Prazo;
                        parcela.Valor = contratoItem.Valor;
                        parcela.BancoReferencia = banco;
                    }

                    contratoItem.DTOParcelaContrato = parcela;
                    dtoContrato.DTOContratoItems.Add(contratoItem);

                    //dtoContrato.Valor = item.
                    
                }
                else
                {
                    dtoContrato.Resultado = "(35) Contrato vencido " + dtoContrato.FimVigencia.Value.ToString("dd/MM/yyyy");
                }

                retorno.Add(dtoContrato);



            }
            return retorno;
        }


    }
}
