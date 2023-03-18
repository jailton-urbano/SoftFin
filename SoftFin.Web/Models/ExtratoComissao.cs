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
using System.Web.Mvc;

namespace SoftFin.Web.Models
{
    public class ExtratoComissao
    {
        [Key]
        public int id { get; set; }

        [Display(Name = "Estabelecimento"), Required(ErrorMessage = "*")]
        public int estabelecimento_id { get; set; }

        [JsonIgnore,ForeignKey("estabelecimento_id")]
        public virtual Estabelecimento Estabelecimento { get; set; }

        [Display(Name = "Seguradora"), StringLength(100)]
        public string Seguradora { get; set; }

        [Display(Name = "Extrato"), StringLength(100)]
        public string Extrato { get; set; }

        [Display(Name = "Data Extrato")]
        public DateTime? DataExtrato { get; set; }

        [Display(Name = "Data Crédito")]
        public DateTime? DataCredito { get; set; }

        [Display(Name = "Data Apropriação")]
        public DateTime? DataApropriacao { get; set; }

        [Display(Name = "Comissão Bruta")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal ComissaoBruta { get; set; }

        [Display(Name = "IRRF")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal IRRF { get; set; }

        [Display(Name = "ISS")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal ISS { get; set; }

        [Display(Name = "Comissão Liquida")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal ComissaoLiquida { get; set; }

        [Display(Name = "Observação"), StringLength(100)]
        public string Observacao { get; set; }


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
                    db.ExtratoComissao.Remove(obj);
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
        public bool Alterar(ExtratoComissao obj, ParamBase pb)
        {
            DbControle db = new DbControle();

            var objAux = ObterPorId(obj.id, pb);
            if (objAux == null)
                return false;
            else
            {
                new LogMudanca().Incluir(obj, objAux, "", db, pb);
                db.Entry(obj).State = EntityState.Modified;
                db.SaveChanges();

                return true;
            }
        }
        private bool validaExistencia(DbControle db, ExtratoComissao obj)
        {
            return (false);
        }
        public bool Incluir(ParamBase pb)
        {
            return Incluir(this, pb);
        }
        public bool Incluir(ExtratoComissao obj, ParamBase pb)
        {
            DbControle db = new DbControle();

            if (validaExistencia(db, obj))
                return false;
            else
            {
                new LogMudanca().Incluir(obj, "", "",db, pb);

                db.Set<ExtratoComissao>().Add(obj);
                db.SaveChanges();

                return true;
            }
        }
        public ExtratoComissao ObterPorId(int id, ParamBase pb)
        {
            return ObterPorId(id, null,pb);
        }
        public ExtratoComissao ObterPorId(int id, DbControle db, ParamBase pb)
        {
            int estab = pb.estab_id;
            if (db == null)
                db = new DbControle();

            return db.ExtratoComissao.Where(x => x.id == id && x.estabelecimento_id == estab).FirstOrDefault();
        }
        public List<ExtratoComissao> ObterTodos(ParamBase pb)
        {
            int estab = pb.estab_id;
            DbControle db = new DbControle();
            return db.ExtratoComissao.Where(x => x.estabelecimento_id == estab).ToList();
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

                for (int numItem = 2; numItem < 50000; numItem++)
                {
                    if (string.IsNullOrEmpty(EXtraiTexto(MyWorksheet, numItem, "A")))
                        break;

                    var Seguradora = EXtraiTexto(MyWorksheet, numItem, "A");
                    var Extrato = EXtraiTexto(MyWorksheet, numItem, "B");
                    var DataExtrato = ExtraiData(MyWorksheet, numItem, "C");


                    ExtratoComissao extratoComissao = new ExtratoComissao().ObterPKHExtrato(Seguradora, Extrato, DataExtrato, db,pb);

                    if (extratoComissao == null)
                    {
                        extratoComissao = new ExtratoComissao();
                        extratoComissao.Seguradora = Seguradora;
                        extratoComissao.Extrato = Extrato;
                        extratoComissao.DataExtrato = DataExtrato;
                        db.ExtratoComissao.Add(extratoComissao);
                    }

                    extratoComissao.DataCredito = ExtraiData(MyWorksheet, numItem, "D");
                    extratoComissao.DataApropriacao = ExtraiData(MyWorksheet, numItem, "E");
                    extratoComissao.ComissaoBruta = ExtraiDecimal(MyWorksheet, numItem, "F");
                    extratoComissao.IRRF = ExtraiDecimal(MyWorksheet, numItem, "G");
                    extratoComissao.ISS = ExtraiDecimal(MyWorksheet, numItem, "H");
                    extratoComissao.ComissaoLiquida = ExtraiDecimal(MyWorksheet, numItem, "I");
                    extratoComissao.Observacao = EXtraiTexto(MyWorksheet, numItem, "J");
                    //extratoComissao.Observacao = EXtraiTexto(MyWorksheet, numItem, "J").Substring(0,100);
                    extratoComissao.estabelecimento_id = idempresa;
                    db.SaveChanges();

                    //
                    //BancoMovimentoConfig bancoMovimentoConfig = new BancoMovimento().Obter();
                    //BancoMovimento bancoMovimento = new BancoMovimento();
                    //bancoMovimento.data = extratoComissao.DataExtrato.Value;
                    //bancoMovimento.empresa_id  = pb.empresa_id;
                    //bancoMovimento.IndicadorDeMovimento = "E";
                    //bancoMovimento.valor = ExtraiDecimal(MyWorksheet, numItem, "F"); ;
                    //bancoMovimento.tipoDeMovimento_id = bancoMovimentoConfig.tipoDeMovimentoCPAG_id.Value;
                    //bancoMovimento.tipoDeDocumento_id = bancoMovimentoConfig.tipoDeMovimentoCPAG_id.Value;
                    //bancoMovimento.origemmovimento_id = bancoMovimentoConfig.origemmovimentoCPAG_id.Value;
                    //bancoMovimento.planoDeConta_id = bancoMovimentoConfig.planoDeContaCPAG_id.Value;
                    //bancoMovimento.historico = "EXTRATO " + extratoComissao.Observacao;
                    //bancoMovimento.banco_id = bancoMovimentoConfig.bancoCPAG_id.Value;
                    //bancoMovimento.unidadeDeNegocio_id = 1;
                    //bancoMovimento. = extratoComissao.id;
                    //bancoMovimento.Incluir(bancoMovimento);

                }
                //db.SaveChanges();

            }
        }

        public ExtratoComissao ObterPKHExtrato(string Seguradora, string Extrato, DateTime DataExtrato, DbControle db, ParamBase pb)
        {
            int estab = pb.estab_id;
            if (db == null)
                db = new DbControle();

            return db.ExtratoComissao.Where(x => x.Seguradora == Seguradora 
                                                && x.Extrato == Extrato
                                                && x.DataExtrato == DataExtrato
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
