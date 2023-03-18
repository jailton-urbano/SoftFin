//using SoftFin.Web.Classes;
//using SoftFin.Web.Models;
//using SoftFin.Web.Negocios;
//using SoftFin.Web.Regras;
//using SoftFin.NFSe.DTO;
//using SoftFin.Utils;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Net;
//using System.Net.Http;
//using System.Web.Http;

//namespace SoftFin.API
//{
//    public class VerificaEmissaoNotaController : BaseApi 
//    {
//        // GET api/<controller>
//        public void Get()
//        {
//            DbControle db = new DbControle();

//            //Pega Todas as Empresa com certificado Digital
//            var estabs = new Estabelecimento().ObterTodosComCertificado();
//            foreach (var estab in estabs)
//            {
//                _paramBase.estab_id = estab.id;
//                _paramBase.empresa_id = estab.Empresa_id;
//                //Consulta notas emitidas não atuliazadas
//                var nfs = new NotaFiscal().ObterTodosEmitidosSemConfirmacaoWS(db, estab.id);
//                foreach (var nf in nfs )
//                {
//                    //Chama rotina de pesquisa de notas no web service

//                    var dTONF = new DTONotaFiscal();
//                    var listaNFs = new List<NotaFiscal>();
//                    listaNFs.Add(nf);
//                    new Conversao().ConverterNFEs(dTONF, estab, listaNFs);
//                    var arquivoxml = "";


//                    var uploadPath = System.Web.Hosting.HostingEnvironment.MapPath("~/CertTMP/");
//                    Directory.CreateDirectory(uploadPath);
//                    var nomearquivonovo = Guid.NewGuid().ToString();
//                    string caminhoArquivo = Path.Combine(@uploadPath, nomearquivonovo);
//                    var cert = UtilSoftFin.BuscaCert(estab.id, estab.senhaCertificado, caminhoArquivo, estab.CNPJ);

//                    //resultado = service.Execute(objNF, cert);

//                    var regra = new SoftFin.NFSe.Business.SFConsultaNFSe();

//                    var result = regra.Execute(dTONF, cert);

//                    if (result.Cabecalho.Sucesso.ToLower() == "true")
//                    {
//                        new NotaFiscalCalculos().BaixaNF(nf,
//                            DateTime.Parse(result.NFe.First().DataEmissaoNFe),
//                            result.NFe.First().ChaveNFe.CodigoVerificacao,
//                            int.Parse(result.NFe.First().ChaveNFe.NumeroNFe),
//                            result.NFe.First().StatusNFe,
//                            db,_paramBase);

//                    }
                    
                    
                    
//                    try
//                    {
//                        cert = null;
//                        System.IO.File.Delete(caminhoArquivo);
//                    }
//                    catch
//                    {
//                    }


//                }
//            }
//        }


//    }
//}