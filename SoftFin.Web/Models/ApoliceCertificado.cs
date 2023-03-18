using Newtonsoft.Json;
using OfficeOpenXml;
using SoftFin.Web.Classes;
using SoftFin.Web.Negocios;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;

namespace SoftFin.Web.Models
{
    public class ApoliceCertificado
    {

        [Key]
        public int id { get; set; }

        [Display(Name = "Estabelecimento"), Required(ErrorMessage = "*")]
        public int estabelecimento_id { get; set; }

        [JsonIgnore,ForeignKey("estabelecimento_id")]
        public virtual Estabelecimento Estabelecimento { get; set; }
        
        [Display(Name = "Endosso"),StringLength(100)]
        public string Endosso { get; set; }

        [Display(Name = "CPF/CNPJ"), StringLength(20)]
        public string CPF_CNPJ { get; set; }

        [Display(Name = "Segurado"), StringLength(100)]
        public string Segurado { get; set; }

        [Display(Name = "Premio Liquido")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public Decimal PremioLiquido { get; set; }

        [Display(Name = "Comissão")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public Decimal Comissao { get; set; }

        [Display(Name = "Atendimento"), StringLength(100)]
        public string Atendimento { get; set; }

        [Display(Name = "Login"), StringLength(100)]
        public string Login { get; set; }

        [Display(Name = "Seguradora"), StringLength(100)]
        public string Seguradora { get; set; }

        [Display(Name = "Data Emissão")]
        public DateTime data_emissao { get; set; }

        [Display(Name = "Apolice Certificado"), StringLength(100)]
        public string Apolice_Certificado { get; set; }


        public bool Excluir(int id, ref string erro, ParamBase pb)
        {
            try
            {
                
                int estab = pb.estab_id;
                DbControle db = new DbControle();
                var obj = ObterPorId(id, db,pb);
                if (obj == null)
                {
                    erro = "Registro não encontrado";
                    return false;
                }
                else
                {
                    new LogMudanca().Incluir(obj, "", "",db, pb);
                    db.ApoliceCertificado.Remove(obj);
                    db.SaveChanges();
                    return true;
                }
            }

            catch (Exception ex)
            {
                if (ex.InnerException.InnerException.Message.IndexOf("The DELETE statement conflicted with the REFERENCE constraint") > -1)
                {
                    erro = "Registro esta relacionado com outro cadastro";
                    return false;
                }
                else
                {
                    throw ex;
                }
            }
        }
        public bool Alterar(ApoliceCertificado obj, ParamBase pb)
        {
            DbControle db = new DbControle();

            var objAux = ObterPorId(obj.id,pb);
            if (objAux == null)
                return false;
            else
            {
                new LogMudanca().Incluir(obj, objAux, "",db, pb);
                db.Entry(obj).State = EntityState.Modified;
                db.SaveChanges();

                return true;
            }
        }
        private bool validaExistencia(DbControle db, ApoliceCertificado obj)
        {
            return (false);
        }
        public bool Incluir(ParamBase pb)
        {
            return Incluir(this, pb);
        }
        public bool Incluir(ApoliceCertificado obj, ParamBase pb)
        {
            DbControle db = new DbControle();

            if (validaExistencia(db, obj))
                return false;
            else
            {
                new LogMudanca().Incluir(obj, "", "",db, pb);

                db.Set<ApoliceCertificado>().Add(obj);
                db.SaveChanges();

                return true;
            }
        }
        public ApoliceCertificado ObterPorId(int id, ParamBase pb)
        {
            return ObterPorId(id, null,pb);
        }
        public ApoliceCertificado ObterPorId(int id, DbControle db, ParamBase pb)
        {
            int idempresa = pb.estab_id;
            if (db == null)
                db = new DbControle();

            return db.ApoliceCertificado.Where(x => x.id == id && x.estabelecimento_id == idempresa).FirstOrDefault();
        }
        public List<ApoliceCertificado> ObterTodos(ParamBase pb)
        {
            int idempresa = pb.estab_id;
            DbControle db = new DbControle();
            return db.ApoliceCertificado.Where(x => x.estabelecimento_id == idempresa).ToList();
        }

        public void ImportaExcel(string Arquivo, ParamBase pb)
        {

            FileInfo fileInfo = new FileInfo(Arquivo);
            using (ExcelPackage MyExcel = new ExcelPackage(fileInfo))
            {
                ExcelWorksheet MyWorksheet = MyExcel.Workbook.Worksheets[1];

                DbControle db = new DbControle();
                int idempresa = pb.estab_id;
                string usuario = Acesso.UsuarioLogado();

                int conta = 0;
                for (int numItem = 2; numItem < 50000; numItem++)
                {
                    if (string.IsNullOrEmpty(EXtraiTexto(MyWorksheet, numItem,"A")))
                        break;


                    var Seguradora = EXtraiTexto(MyWorksheet, numItem,"D");
                    var data = ExtraiData(MyWorksheet, numItem,"K");
                    var apolice = EXtraiTexto(MyWorksheet, numItem, "O");

                    ApoliceCertificado apoliceCertificado = new ApoliceCertificado().ObterPKHExtrato(Seguradora, data, apolice, db,pb);

                    if (apoliceCertificado == null)
                    {
                        apoliceCertificado = new ApoliceCertificado();
                        apoliceCertificado.Seguradora = Seguradora;
                        apoliceCertificado.Apolice_Certificado = apolice;
                        apoliceCertificado.data_emissao = data;
                        db.ApoliceCertificado.Add(apoliceCertificado);
                    }

                    
                    apoliceCertificado.Endosso = EXtraiTexto(MyWorksheet, numItem,"P");
                    apoliceCertificado.Comissao = ExtraiDecimal(MyWorksheet, numItem, "J");
                    apoliceCertificado.CPF_CNPJ = EXtraiTexto(MyWorksheet, numItem, "T");
                    apoliceCertificado.PremioLiquido = ExtraiDecimal(MyWorksheet, numItem, "I");
                    apoliceCertificado.Segurado = EXtraiTexto(MyWorksheet, numItem, "S");
                    apoliceCertificado.Seguradora = EXtraiTexto(MyWorksheet, numItem, "D");
                    //apoliceCertificado.Atendimento = EXtraiTexto(MyWorksheet, numItem, "T");
                    apoliceCertificado.Login = usuario;
                    apoliceCertificado.estabelecimento_id = idempresa;

                    conta += 1;
                    if (conta == 1000)
                    {
                        conta = 0;
                        db.SaveChanges();
                    }
                }
                db.SaveChanges();
                
            }
        }

        private ApoliceCertificado ObterPKHExtrato(string Seguradora, DateTime data, string apolice, DbControle db, ParamBase pb)
        {
            int idempresa = pb.estab_id;
            if (db == null)
                db = new DbControle();

            return db.ApoliceCertificado.Where(x => x.estabelecimento_id == idempresa && 
                                            x.data_emissao == data &&
                                            x.Seguradora == Seguradora &&
                                            x.Apolice_Certificado == apolice).FirstOrDefault();
        }



        private static string EXtraiTexto(ExcelWorksheet MyWorksheet, int numItem, string LetraColuna)
        {
            try
            {
                return MyWorksheet.Cells[LetraColuna + numItem.ToString()].Value.ToString();
            }
            catch
            {
                return "";
            }
        }

        private static decimal ExtraiDecimal(ExcelWorksheet MyWorksheet, int numItem, string LetraColuna)
        {
            try
            {
                return decimal.Parse(MyWorksheet.Cells[LetraColuna + numItem.ToString()].Value.ToString());
            }
            catch
            {
                return 0;
            }
        }



        private static DateTime ExtraiData(ExcelWorksheet MyWorksheet, int numItem, string LetraColuna)
        {
            try
            {
                return DateTime.Parse(MyWorksheet.Cells[LetraColuna + numItem.ToString()].Value.ToString());
            }
            catch
            {
                return DateTime.Now;
            }
        }    



        //public static string GeraArquivoPCNC(List<VW_INDICADOR> colecaoIdentificadorDados, List<TB_PCNC> colecaoIdentificador, FileInfo existingFile, string arquivoPara)
        //{
        //    FileInfo fNewFile = new FileInfo(arquivoPara);
        //    using (ExcelPackage MyExcel = new ExcelPackage(existingFile))
        //    {
        //        ExcelWorksheet MyWorksheet = MyExcel.Workbook.Worksheets[1];

        //        DadosIdentificador(colecaoIdentificadorDados, MyWorksheet, "A.1", "D4");
        //        DadosIdentificador(colecaoIdentificadorDados, MyWorksheet, "A.1", "D5");
        //        DadosIdentificador(colecaoIdentificadorDados, MyWorksheet, "A.2", "D6");
        //        DadosIdentificador(colecaoIdentificadorDados, MyWorksheet, "A.5", "D7");
        //        DadosIdentificador(colecaoIdentificadorDados, MyWorksheet, "A.6", "D8");
        //        DadosIdentificador(colecaoIdentificadorDados, MyWorksheet, "1.1", "D9");
        //        DadosIdentificador(colecaoIdentificadorDados, MyWorksheet, "A.7", "D10");
        //        DadosIdentificador(colecaoIdentificadorDados, MyWorksheet, "A.8", "D11");

        //        DadosIdentificador(colecaoIdentificadorDados, MyWorksheet, "B.3", "D12");
        //        MyWorksheet.Cells["D13"].Value = DateTime.Now.ToShortDateString();
        //        DadosIdentificador(colecaoIdentificadorDados, MyWorksheet, "A.9", "D14");
        //        DadosIdentificador(colecaoIdentificadorDados, MyWorksheet, "A.3", "D15");
        //        DadosIdentificador(colecaoIdentificadorDados, MyWorksheet, "A.4", "D16");

        //        int numItem = 1;
        //        int num = 28;

        //        foreach (var item in colecaoIdentificador)
        //        {

        //            MyWorksheet.Cells["A" + num.ToString()].Value = numItem;
        //            MyWorksheet.Cells["B" + num.ToString()].Value = item.TB_RESULTADO.TB_PERGUNTA.TB_INDICADOR.DESCRICAO;
        //            MyWorksheet.Cells["C" + num.ToString()].Value = item.TB_RESULTADO.TB_PERGUNTA.DESCRICAO;
        //            MyWorksheet.Cells["D" + num.ToString()].Value = item.EVIDENCIAS;
        //            MyWorksheet.Cells["E" + num.ToString()].Value = item.ACAOCORRETIVA;
        //            MyWorksheet.Cells["F" + num.ToString()].Value = item.PRAZO;
        //            MyWorksheet.Cells["G" + num.ToString()].Value = item.MONITORAMENTO;
        //            MyWorksheet.Cells["H" + num.ToString()].Value = item.OBSERVACAO;

        //            numItem += 1;
        //            num += 1;
        //        }

        //        MyExcel.SaveAs(fNewFile);
        //    }
        //    return arquivoPara;
        //}
    }
}