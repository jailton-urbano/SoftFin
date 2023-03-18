using Newtonsoft.Json;
using OfficeOpenXml;
using SoftFin.Web.Classes;
using SoftFin.Web.Negocios;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;

namespace SoftFin.Web.Models
{
    public class ParcelaComissao
    {
        [Key]
        public int id { get; set; }

        [Display(Name = "Estabelecimento"), Required(ErrorMessage = "*")]
        public int estabelecimento_id { get; set; }

        [JsonIgnore,ForeignKey("estabelecimento_id")]
        public virtual Estabelecimento Estabelecimento { get; set; }


        [Display(Name = "EN1"),StringLength(100)]
        public string en1 { get; set; }

        [Display(Name = "EN2"), StringLength(100)]
        public string en2 { get; set; }

        [Display(Name = "EN3"), StringLength(100)]
        public string en3 { get; set; }

        [Display(Name = "Seguradora"), StringLength(100)]
        public string seguradora { get; set; }

        [Display(Name = "Familia"), StringLength(100)]
        public string familia { get; set; }

        [Display(Name = "Produto"), StringLength(100)]
        public string produto { get; set; }

        [Display(Name = "Tipo"), StringLength(100)]
        public string tipo { get; set; }

        [Display(Name = "Extrato"), StringLength(100)]
        public string extrato { get; set; }

        [Display(Name = "Data do Extrato")]
        public DateTime? data_extrato { get; set; }

        [Display(Name = "Data do Crédito")]
        public DateTime? data_credito { get; set; }

        [Display(Name = "Percela")]
        public int parcela { get; set; }

        [Display(Name = "Comissão Bruta")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal comissao_bruta { get; set; }


        public bool Excluir(int id, ref string erro, ParamBase pb)
        {
            try
            {
                int estab = pb.estab_id;
                DbControle db = new DbControle();
                var obj = ObterPorId(id, db, pb);
                if (obj == null)
                {
                    erro = "Registro não encontrado";
                    return false;
                }
                else
                {
                    new LogMudanca().Incluir(obj, "", "",db, pb);
                    db.ParcelaComissao.Remove(obj);
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
        public bool Alterar(ParcelaComissao obj, ParamBase pb)
        {
            DbControle db = new DbControle();

            var objAux = ObterPorId(obj.id, pb);
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
        private bool validaExistencia(DbControle db, ParcelaComissao obj)
        {
            return (false);
        }
        public bool Incluir(ParamBase pb)
        {
            return Incluir(this, pb);
        }
        public bool Incluir(ParcelaComissao obj, ParamBase pb)
        {
            DbControle db = new DbControle();

            if (validaExistencia(db, obj))
                return false;
            else
            {
                new LogMudanca().Incluir(obj, "", "",db, pb);

                db.Set<ParcelaComissao>().Add(obj);
                db.SaveChanges();

                return true;
            }
        }
        public ParcelaComissao ObterPorId(int id, ParamBase pb)
        {
            return ObterPorId(id, null, pb);
        }
        public ParcelaComissao ObterPorId(int id, DbControle db, ParamBase pb)
        {
            int idempresa = pb.estab_id; 
            if (db == null)
                db = new DbControle();

            return db.ParcelaComissao.Where(x => x.id == id && x.estabelecimento_id == idempresa).FirstOrDefault();
        }
        public List<ParcelaComissao> ObterTodos(ParamBase pb)
        {
            int idempresa = pb.estab_id;
            DbControle db = new DbControle();
            return db.ParcelaComissao.Where(x => x.estabelecimento_id == idempresa).ToList();
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
                    if (string.IsNullOrEmpty(EXtraiTexto(MyWorksheet, numItem, "A")))
                        break;

                    var seguradora = EXtraiTexto(MyWorksheet, numItem, "D");
                    var extrato = EXtraiTexto(MyWorksheet, numItem, "H");
                    var data_extrato = ExtraiData(MyWorksheet, numItem, "I");

                    ParcelaComissao parcelaComissao = new ParcelaComissao().ObterPKHExtrato(seguradora, extrato, data_extrato, db, pb);

                    if (parcelaComissao == null)
                    {
                        parcelaComissao = new ParcelaComissao();
                        parcelaComissao.seguradora = EXtraiTexto(MyWorksheet, numItem, "D");
                        parcelaComissao.extrato = EXtraiTexto(MyWorksheet, numItem, "H");
                        parcelaComissao.data_extrato = ExtraiData(MyWorksheet, numItem, "I");
                        db.ParcelaComissao.Add(parcelaComissao);
                    }

                    parcelaComissao.en1 = EXtraiTexto(MyWorksheet, numItem, "A");
                    parcelaComissao.en2 = EXtraiTexto(MyWorksheet, numItem, "B");
                    parcelaComissao.en3 = EXtraiTexto(MyWorksheet, numItem, "C");
                    parcelaComissao.familia = EXtraiTexto(MyWorksheet, numItem, "E");
                    parcelaComissao.produto = EXtraiTexto(MyWorksheet, numItem, "F");
                    parcelaComissao.tipo = EXtraiTexto(MyWorksheet, numItem, "G");
                    parcelaComissao.data_credito = ExtraiData(MyWorksheet, numItem, "J");
                    parcelaComissao.parcela = ExtraiInt(MyWorksheet, numItem, "K");
                    parcelaComissao.comissao_bruta = ExtraiDecimal(MyWorksheet, numItem, "L");
                    parcelaComissao.estabelecimento_id = idempresa;


                    conta += 1;
                    if (conta == 1000)
                        db.SaveChanges();
                }
                db.SaveChanges();

            }
        }

        private ParcelaComissao ObterPKHExtrato(string seguradora, string extrato, DateTime data_extrato, DbControle db, ParamBase pb)
        {
            int estab = pb.estab_id;
            if (db == null)
                db = new DbControle();

            return db.ParcelaComissao.Where(x => x.seguradora == seguradora
                                                && x.extrato == extrato
                                                && x.data_extrato == data_extrato
                                                && x.estabelecimento_id == estab).FirstOrDefault();
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
        private static int ExtraiInt(ExcelWorksheet MyWorksheet, int numItem, string LetraColuna)
        {
            try
            {
                return int.Parse(MyWorksheet.Cells[LetraColuna + numItem.ToString()].Value.ToString());
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
    }
}