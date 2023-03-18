using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using SoftFin.GestorProcessos.Comum.Param;
using SoftFin.GestorProcessos.Models;

namespace SoftFin.GestorProcessos.Helper
{
    public class SQLManipulation : BaseSQLConexao
    {
        public bool ExisteTabela(string tabela)
        {
            // Provide the query string with a parameter placeholder.
            bool retorno = false;
            string queryString = "DECLARE @EXISTETABELA BIT;";
            queryString += "SET @EXISTETABELA = (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = @tabela)";
            queryString += "SELECT @EXISTETABELA; ";

            using (SqlConnection connection =
                new SqlConnection(_stringConecao))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@tabela", tabela);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    reader.Read();
                    retorno = !(reader[0] == System.DBNull.Value);
                    reader.Close();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                //Console.ReadLine();
            }

            return retorno;
        }

        internal string BuscaJSONNovo(ParamProcesso paramProcesso1)
        {
            var retorno = "";


            string queryString = "EXEC PRCSF_NOVO_" + paramProcesso1.CodigoAtividade;
            using (SqlConnection connection =
                new SqlConnection(_stringConecao))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                reader.Read();
                retorno = reader[0].ToString();


            }

             return retorno;
        }

        internal string BuscaJSONNovoTabela(ParamProcesso paramProcesso, 
            string tabela, 
            string ordem, 
            int paginaatual, 
            int totalporpagina, 
            ref int qtdRegistro)
        {
            var retorno = "";
            StringBuilder sbquery = new StringBuilder();

            sbquery.AppendLine("SELECT COUNT(*) FROM " + tabela);

            using (SqlConnection connection =
                new SqlConnection(_stringConecao))
            {
                SqlCommand command = new SqlCommand(sbquery.ToString(), connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    qtdRegistro = int.Parse(reader[0].ToString());
                }
                reader.Close();
                connection.Close();
            }

            int registroInicial = 0;
            int registroFinal = totalporpagina;

            if (paginaatual > 1)
            {
                registroFinal = paginaatual * totalporpagina;
                registroInicial = registroFinal - totalporpagina;
            }

            sbquery = new StringBuilder();

            sbquery.AppendLine("EXEC PRCSF_OBTER_TABELA_" + tabela + " " + registroInicial.ToString() + "," + registroFinal.ToString() + " ");

            using (SqlConnection connection =
                new SqlConnection(_stringConecao))
            {
                SqlCommand command = new SqlCommand(sbquery.ToString(), connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                    retorno = reader[0].ToString();

                reader.Close();
                connection.Close();
            }
            return retorno;
        }

        internal List<string> BuscaJSONExistente(ParamProcesso paramProcesso)
        {
            List<string> retorno = new List<string>();


            string queryString = "EXEC PRCSF_OBTER_" + paramProcesso.CodigoAtividade + " @PROCESSOEXECUCAO_ID ";
            using (SqlConnection connection =
                new SqlConnection(_stringConecao))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@PROCESSOEXECUCAO_ID", paramProcesso.CodigoProcessoAtual);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        retorno.Add(reader[i].ToString());
                    }
                    
                }
                
            }

            return retorno;
        }


        internal string SalvaJson(ParamProcesso paramProcesso, string json)
        {
            var retorno = "";


            string queryString = "EXEC PRCSF_INCLUIR_" + paramProcesso.CodigoAtividade + " @JSON, @PROCESSOEXECUCAO_ID";
            using (SqlConnection connection =
                new SqlConnection(_stringConecao))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@JSON", json);
                command.Parameters.AddWithValue("@PROCESSOEXECUCAO_ID", paramProcesso.CodigoProcessoAtual);
                connection.Open();
                var reader = command.ExecuteNonQuery();

            }

            return retorno;
        }

        internal string SalvaTabelaJson(ParamProcesso paramProcesso, string json)
        {
            var retorno = "";
            

            string queryString = "EXEC PRCSF_SALVAR_TABELA_" + paramProcesso.Tabela + " @JSON";
            using (SqlConnection connection =
                new SqlConnection(_stringConecao))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@JSON", json);
                connection.Open();
                var reader = command.ExecuteNonQuery();

            }

            return retorno;
        }

        internal string ExcluiTabelaJson(ParamProcesso paramProcesso, int Id)
        {
            var retorno = "";


            string queryString = "DELETE FROM " + paramProcesso.Tabela + " WHERE ID = " + Id.ToString();
            using (SqlConnection connection =
                new SqlConnection(_stringConecao))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
             
                connection.Open();
                var reader = command.ExecuteNonQuery();

            }

            return retorno;
        }

        public List<Dictionary<string, string>> ExisteCampo(string tabela, string campo)
        {
            // Provide the query string with a parameter placeholder.
            List<Dictionary<string, string>> retorno = null;


            string queryString = "SELECT DATA_TYPE, CHARACTER_MAXIMUM_LENGTH FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = @tabela AND COLUMN_NAME = @campo;";


            using (SqlConnection connection =
                new SqlConnection(_stringConecao))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@tabela", tabela);
                command.Parameters.AddWithValue("@campo", campo);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                   
                    if (reader.Read())
                    {
                        retorno = new List<Dictionary<string, string>>();
                        var item = new Dictionary<string, string>();
                        item.Add("DATA_TYPE", reader[0].ToString());
                        retorno.Add( item);
                        var item2 = new Dictionary<string, string>();
                        item2.Add("CHARACTER_MAXIMUM_LENGTH", reader[1].ToString());
                        retorno.Add(item2);
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                 
            }

            return retorno;
        }
                
        public bool CriaTabela(string tabela, bool tabelaAuxiliar)
        {
            // Provide the query string with a parameter placeholder.
            bool retorno = false;
            string queryString = "";

            if (tabelaAuxiliar)
                queryString = "CREATE TABLE dbo." + tabela + " (Id int IDENTITY PRIMARY KEY CLUSTERED); ";
            else
                queryString = "CREATE TABLE dbo." + tabela + " (Id int IDENTITY PRIMARY KEY CLUSTERED, ProcessoExecucao_Id uniqueidentifier not null ); ";


            using (SqlConnection connection =
                new SqlConnection(_stringConecao))
            {
                SqlCommand command = new SqlCommand(queryString, connection);

                try
                {
                    connection.Open();
                    var reader = command.ExecuteNonQuery();
                    retorno = (reader == -1);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                
            }

            return retorno;
        }
        
        public bool CriaCampo(string tabela, string campo, string tipo)
        {
            // Provide the query string with a parameter placeholder.
            bool retorno = false;
            string queryString = "ALTER TABLE dbo." + tabela + " ADD " + campo + " " + tipo +"; ";


            using (SqlConnection connection =
                new SqlConnection(_stringConecao))
            {
                SqlCommand command = new SqlCommand(queryString, connection);

                try
                {
                    connection.Open();
                    var reader = command.ExecuteNonQuery();
                    retorno = (reader == -1);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                
            }

            return retorno;
        }


        public bool CriaIndice(string tabela, string campo, string FkTabela)
        {
            // Provide the query string with a parameter placeholder.
            bool retorno = false;
            StringBuilder queryString = new StringBuilder();
            queryString.AppendLine("ALTER TABLE dbo." + tabela + " ADD CONSTRAINT " + " FKSF_" + tabela + "_" + campo + " FOREIGN KEY (" + campo + ") REFERENCES " + FkTabela +"(Id); ");
            
            using (SqlConnection connection =
                new SqlConnection(_stringConecao))
            {
                SqlCommand command = new SqlCommand(queryString.ToString(), connection);

                try
                {
                    connection.Open();
                    var reader = command.ExecuteNonQuery();
                    retorno = (reader == -1);
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }

            return retorno;
        }

        public bool ExcluiIndice(string tabela, string campo, string FkTabela)
        {
            // Provide the query string with a parameter placeholder.
            bool retorno = false;
            StringBuilder queryString = new StringBuilder();
            queryString.AppendLine("ALTER TABLE dbo." + tabela + " DROP CONSTRAINT  " + "FKSF_" + tabela + "_" + campo);

            using (SqlConnection connection =
                new SqlConnection(_stringConecao))
            {
                SqlCommand command = new SqlCommand(queryString.ToString(), connection);

                try
                {
                    connection.Open();
                    var reader = command.ExecuteNonQuery();
                    retorno = (reader == -1);
                }
                catch (Exception ex)
                {
                    //throw ex;
                }

            }

            return retorno;
        }

        public bool AlterarCampo(string tabela, string campo, string tipo)
        {
            // Provide the query string with a parameter placeholder.
            bool retorno = false;
            string queryString = "ALTER TABLE dbo." + tabela + " ALTER COLUMN  " + campo + " " + tipo + "; ";


            using (SqlConnection connection =
                new SqlConnection(_stringConecao))
            {
                SqlCommand command = new SqlCommand(queryString, connection);

                try
                {
                    connection.Open();
                    var reader = command.ExecuteNonQuery();
                    retorno = (reader == -1);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                
            }

            return retorno;
        }



        public bool ExcluiCampo(string tabela, string campo, string tipo)
        {
            // Provide the query string with a parameter placeholder.
            bool retorno = false;
            string queryString = "ALTER TABLE dbo." + tabela + " DROP COLUMN " + campo + "; ";


            using (SqlConnection connection =
                new SqlConnection(_stringConecao))
            {
                SqlCommand command = new SqlCommand(queryString, connection);

                try
                {
                    connection.Open();
                    var reader = command.ExecuteNonQuery();
                    retorno = (reader == -1);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                
            }

            return retorno;
        }

        public bool DropProcedure(string nome)
        {
            // Provide the query string with a parameter placeholder.
            bool retorno = false;
            StringBuilder queryString = new StringBuilder();

            queryString.AppendLine("DROP PROCEDURE [dbo].[" + nome + "];");
            

            using (SqlConnection connection =
                new SqlConnection(_stringConecao))
            {
                SqlCommand command = new SqlCommand(queryString.ToString(), connection);

                try
                {
                    connection.Open();
                    var reader = command.ExecuteNonQuery();
                    retorno = (reader == -1);
                }
                catch (Exception ex)
                {
                    //throw ex;
                }

            }

            return retorno;
        }


        public bool CriarProcedureNovo(string atividade, List<AtividadeVisao> atividadeVisao, DBGPControle db)
        {
            // Provide the query string with a parameter placeholder.
            bool retorno = false;
            StringBuilder queryString = new StringBuilder();
            var nomeProcedure = "PRCSF_NOVO_" + atividade;
            DropProcedure(nomeProcedure);
            queryString.AppendLine(" CREATE PROCEDURE [dbo].[" + nomeProcedure + "] ");
            queryString.AppendLine(" AS ");
            queryString.AppendLine(" BEGIN ");
            queryString.AppendLine(" SET NOCOUNT ON; ");

            foreach (var item in atividadeVisao)
            {
                var nomeTabela = item.Visao.Tabela.Nome;
                var campos = db.TabelaCampo.Where(p => p.Tabela_Id == item.Visao.IdTabela).ToList();
                CriaSQLDeclare(queryString, nomeTabela, campos);
            }

            queryString.AppendLine(" SELECT ");

            StringBuilder queryStringCampos = new StringBuilder();

            foreach (var item in atividadeVisao)
            {
                var nomeTabela = item.Visao.Tabela.Nome;

                var campos = db.TabelaCampo.Where(p => p.Tabela_Id == item.Visao.IdTabela);


                foreach (var itemX in campos)
                {
                    if (queryStringCampos.Length != 0)
                        queryStringCampos.Append(",");

                    queryStringCampos.Append(" @" + nomeTabela + "_" + itemX.Campo + " AS '" + nomeTabela + "." + itemX.Campo + "'");
                }


            }

            if (queryStringCampos.Length == 0)
                return true;

            queryString.AppendLine(queryStringCampos.ToString());
            queryString.AppendLine(" FOR JSON PATH; ");
            queryString.AppendLine(" END ");

            using (SqlConnection connection =
                new SqlConnection(_stringConecao))
            {
                SqlCommand command = new SqlCommand(queryString.ToString(), connection);

                try
                {
                    connection.Open();
                    var reader = command.ExecuteNonQuery();
                    retorno = (reader == -1);
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }

            return retorno;
        }

        private static void CriaSQLDeclare(StringBuilder queryString, string nomeTabela, List<TabelaCampo> campos)
        {
            foreach (var itemX in campos)
            {
                var tipoCampo = itemX.TipoCampo.TipoBancoDados.Replace("#numpos", itemX.TamanhoCampo).Replace("#numpre", itemX.Precisao.ToString());
                var sqlDefault = "";

                if (!string.IsNullOrEmpty(itemX.TipoCampo.SQLDefault))
                    sqlDefault = " = " + itemX.TipoCampo.SQLDefault;

                if (!string.IsNullOrEmpty(itemX.SQLDefault))
                    sqlDefault = " = " + itemX.SQLDefault;

                queryString.AppendLine(" DECLARE @" + nomeTabela + "_" + itemX.Campo + " " + tipoCampo + sqlDefault + "; ");
            }
        }

        private static void CriaSQLInsert(StringBuilder queryString, string nomeTabela, List<TabelaCampo> campos)
        {
            queryString.AppendLine("INSERT INTO " + nomeTabela + "(");

            StringBuilder queryStringA = new StringBuilder();
            StringBuilder queryStringB = new StringBuilder();

            foreach (var item in campos)
            {
                if (!item.Campo.ToUpper().Equals("ID"))
                {
                    if (queryStringA.Length != 0)
                    {
                        queryStringA.Append(",");
                    }
                    if (queryStringB.Length != 0)
                    {
                        queryStringB.Append(",");
                    }
                    queryStringA.Append(item.Campo);
                    queryStringB.Append("@" + nomeTabela + "_" + item.Campo);
                }
            }
            queryString.AppendLine(queryStringA.ToString() + ") values (" + queryStringB.ToString() + "); ");
        }

        private static void CriaSQLUpdate(StringBuilder queryString, string nomeTabela, List<TabelaCampo> campos)
        {
            queryString.AppendLine("UPDATE " + nomeTabela + " SET");

            StringBuilder queryStringA = new StringBuilder();


            foreach (var item in campos)
            {
                if (!item.Campo.ToUpper().Equals("ID"))
                {
                    if (queryStringA.Length != 0)
                    {
                        queryStringA.Append(",");
                    }

                    queryStringA.Append(item.Campo + " = @" + nomeTabela + "_" + item.Campo);
                }
            }
            queryString.AppendLine(queryStringA.ToString() + " WHERE Id = @" + nomeTabela + "_Id");
        }
        private static void CriaSQLFetch(StringBuilder queryString, List<VisaoCampo> camposVisao, string nomeTabela, List<TabelaCampo> campos)
        {
            StringBuilder queryStringINTO = new StringBuilder();
            foreach (var itemX in campos)
            {
                var camposVisaoA = camposVisao.Where(p => p.IdTabelaCampo == itemX.Id).FirstOrDefault();
                if (camposVisaoA == null)
                {
                    if (queryStringINTO.Length != 0)
                        queryStringINTO.AppendLine(",");
                    queryStringINTO.AppendLine(" @" + nomeTabela + "_" + itemX.Campo + " ");
                }
                else
                {

                        var geracampo = true;
                        geracampo = camposVisaoA.Transferivel;
                        if (geracampo == true)
                        {
                            if (queryStringINTO.Length != 0)
                                queryStringINTO.Append(",");
                            queryStringINTO.AppendLine(" @" + nomeTabela + "_" + itemX.Campo + " ");
                        }
                    
                }
            }


            queryString.AppendLine("FETCH NEXT FROM @CURSOR_" + nomeTabela + " INTO " + queryStringINTO);

        }

        private static void CriaSQLValidacao(StringBuilder queryString, string nomeTabela, List<TabelaCampo> campos, List<VisaoCampo> camposVisao)
        {
            foreach (var itemX in campos)
            {
                var camposVisaoA = camposVisao.Where(p => p.IdTabelaCampo == itemX.Id).FirstOrDefault();
                if (camposVisaoA == null)
                {
                    var tipoCampo = itemX.TipoCampo;

                    if (itemX.Obrigatorio)
                    {
                        CriaSQLValidacaoCondicao(queryString, nomeTabela, itemX);
                    }
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(camposVisaoA.PadraoSalva))
                    {
                        var geracampo = true;
                        geracampo = camposVisaoA.Transferivel;
                        if (geracampo == true)
                        {
                            if (itemX.Obrigatorio)
                            {
                                CriaSQLValidacaoCondicao(queryString, nomeTabela, itemX);
                            }
                        }
                    }
                }
            }
        }

        internal string BuscaJSONNovoTabela(ParamProcesso paramProcesso, string tabela, string order, int page, int limit, ref object total)
        {
            throw new NotImplementedException();
        }

        private static void CriaSQLValidacaoCondicao(StringBuilder queryString, string nomeTabela, TabelaCampo itemX)
        {
            var campo = nomeTabela + "_" + itemX.Campo;
            switch (itemX.TipoCampo.TipoBancoDados.ToUpper())
            {
                case "INT":
                case "DECIMAL":
                    queryString.AppendLine(" IF @" + campo + "  = 0 ");
                    queryString.AppendLine("    BEGIN ");
                    queryString.AppendLine("       RAISERROR('Campo " + campo + " Obrigatorio', 18,-1);");
                    queryString.AppendLine("       RETURN");
                    queryString.AppendLine("    END ");

                    break;

                default:
                    queryString.AppendLine(" IF @" + campo + "  = '' ");
                    queryString.AppendLine("    BEGIN ");
                    queryString.AppendLine("       RAISERROR('Campo " + campo + " Obrigatorio', 18,-1);");
                    queryString.AppendLine("       RETURN");
                    queryString.AppendLine("    END ");
                    break;
            }
        }

        public bool CriarProcedureIncluir(string atividade, List<AtividadeVisao> atividadeVisao, DBGPControle db)
        {
            // Provide the query string with a parameter placeholder.
            bool retorno = false;
            StringBuilder queryString = new StringBuilder();
            var nomeProcedure = "PRCSF_INCLUIR_" + atividade;
            DropProcedure(nomeProcedure);

            queryString.AppendLine("CREATE PROCEDURE [dbo].[" + nomeProcedure + "] (@JSON VARCHAR(MAX), @PROCESSOEXECUCAO_ID uniqueidentifier) ");
            queryString.AppendLine(" AS ");
            queryString.AppendLine(" BEGIN  ");
            queryString.AppendLine(" SET NOCOUNT ON;");


            foreach (var item in atividadeVisao.OrderBy(p => p.Ordem))
            {
                var campos = db.TabelaCampo.Where(p => p.Tabela_Id == item.Visao.IdTabela).ToList();
                var camposVisao = db.VisaoCampo.Where(p => p.IdVisao == item.IdVisao);

                StringBuilder queryStringCamposA = new StringBuilder();
                CriaSQLDeclare(queryStringCamposA, item.Visao.Tabela.Nome, campos);

                queryStringCamposA.AppendLine(" DECLARE @CURSOR_" + item.Visao.Tabela.Nome + " CURSOR; ");
                queryString.AppendLine(queryStringCamposA.ToString());
            }
                       

            foreach (var item in atividadeVisao.OrderBy(p => p.Ordem))
            {
                var campos = db.TabelaCampo.Where(p => p.Tabela_Id == item.Visao.IdTabela).ToList();
                var camposVisao = db.VisaoCampo.Where(p => p.IdVisao == item.IdVisao).ToList();

                StringBuilder queryStringCamposB = new StringBuilder();
                StringBuilder queryStringCamposC = new StringBuilder();

                GeraCamposProcedureIncluirFrom(campos, camposVisao, queryStringCamposB);
                GeraCamposProcedureIncluirfromJson(campos, camposVisao, queryStringCamposC);

                queryString.AppendLine(" SET @CURSOR_" + item.Visao.Tabela.Nome + " = CURSOR FOR ");
                queryString.AppendLine(" SELECT ");
                queryString.AppendLine(queryStringCamposB.ToString());
                queryString.AppendLine(" FROM ");
                if (item.Visao.TipoVisao.Descricao == "Collection")
                    queryString.AppendLine(" OPENJSON ( @JSON, 'lax $.Lista" + item.Visao.Tabela.Nome + "' ) ");
                else
                    queryString.AppendLine(" OPENJSON ( @JSON, 'lax $." + item.Visao.Tabela.Nome + "' ) ");
                queryString.AppendLine(" WITH ( ");
                queryString.AppendLine(queryStringCamposC.ToString());
                queryString.AppendLine(" ); ");
                queryString.AppendLine(" OPEN @CURSOR_" + item.Visao.Tabela.Nome + ";");
                CriaSQLFetch(queryString, camposVisao, item.Visao.Tabela.Nome, campos);
                queryString.AppendLine(" WHILE @@FETCH_STATUS = 0 ");
                queryString.AppendLine("    BEGIN ");
                CriaSQLValidacao(queryString,  item.Visao.Tabela.Nome, campos, camposVisao);
                CriaSQLFetch(queryString, camposVisao, item.Visao.Tabela.Nome, campos);
                queryString.AppendLine("    END ");
                queryString.AppendLine(" CLOSE @CURSOR_" + item.Visao.Tabela.Nome + ";");

            }


            queryString.AppendLine(" BEGIN TRY");
            queryString.AppendLine("    BEGIN TRANSACTION;");

            foreach (var item in atividadeVisao.OrderByDescending(p => p.Ordem))
            {
                queryString.AppendLine(" DELETE FROM " + item.Visao.Tabela.Nome + " WHERE ProcessoExecucao_Id = @PROCESSOEXECUCAO_ID;");
            }

            foreach (var item in atividadeVisao.OrderBy(p => p.Ordem))
            {
                var campos = db.TabelaCampo.Where(p => p.Tabela_Id == item.Visao.IdTabela).ToList();
                var camposVisao = db.VisaoCampo.Where(p => p.IdVisao == item.IdVisao).ToList();

                StringBuilder queryStringCamposA = new StringBuilder();
                StringBuilder queryStringCamposB = new StringBuilder();
                StringBuilder queryStringCamposC = new StringBuilder();

                GeraCamposProcedureIncluirSelect(campos, camposVisao, queryStringCamposA);
                GeraCamposProcedureIncluirFrom(campos, camposVisao, queryStringCamposB);
                GeraCamposProcedureIncluirfromJson(campos, camposVisao, queryStringCamposC);

                queryString.AppendLine("INSERT INTO [dbo].[" + item.Visao.Tabela.Nome + "] (");
                queryString.AppendLine(queryStringCamposA.ToString());
                if (!item.Visao.Tabela.CadastroAuxiliar)
                {
                    queryString.AppendLine(", ProcessoExecucao_Id");
                }
                queryString.AppendLine(") SELECT ");
                queryString.AppendLine(queryStringCamposB.ToString());
                if (!item.Visao.Tabela.CadastroAuxiliar)
                {
                    queryString.AppendLine(", @PROCESSOEXECUCAO_ID");
                }
                queryString.AppendLine(" FROM ");
                if (item.Visao.TipoVisao.Descricao == "Collection")
                    queryString.AppendLine("OPENJSON ( @JSON, 'lax $.Lista" + item.Visao.Tabela.Nome + "' ) ");
                else
                    queryString.AppendLine(" OPENJSON ( @JSON, 'lax $." + item.Visao.Tabela.Nome + "' ) ");
                queryString.AppendLine(" WITH ( ");
                queryString.AppendLine(queryStringCamposC.ToString());
                queryString.AppendLine(" ); ");
            }

            queryString.AppendLine("    COMMIT;");
            queryString.AppendLine(" END TRY");

            queryString.AppendLine(" BEGIN CATCH");
            queryString.AppendLine("    ROLLBACK TRANSACTION;");
            queryString.AppendLine(" END CATCH");

            queryString.AppendLine("END");

            using (SqlConnection connection =
                new SqlConnection(_stringConecao))
            {
                SqlCommand command = new SqlCommand(queryString.ToString(), connection);

                try
                {
                    connection.Open();
                    var reader = command.ExecuteNonQuery();
                    retorno = (reader == -1);
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }

            return retorno;
        }

        public bool CriarProcedureTabelaSalvar(string tabela, List<TabelaCampo> tabelaCampo, DBGPControle db)
        {
            // Provide the query string with a parameter placeholder.
            bool retorno = false;
            StringBuilder queryString = new StringBuilder();
            var nomeProcedure = "PRCSF_SALVAR_TABELA_" + tabela;
            DropProcedure(nomeProcedure);

            queryString.AppendLine("CREATE PROCEDURE [dbo].[" + nomeProcedure + "] (@JSON VARCHAR(MAX)) ");
            queryString.AppendLine(" AS ");
            queryString.AppendLine(" BEGIN  ");
            queryString.AppendLine(" SET NOCOUNT ON;");





            StringBuilder queryStringCamposA = new StringBuilder();
            CriaSQLDeclare(queryStringCamposA, tabela, tabelaCampo);

            queryStringCamposA.AppendLine(" DECLARE @CURSOR_" + tabela + " CURSOR; ");
            queryString.AppendLine(queryStringCamposA.ToString());





            StringBuilder queryStringCamposB = new StringBuilder();
            StringBuilder queryStringCamposC = new StringBuilder();
            var visaocampos = new List<VisaoCampo>().ToList();
            GeraCamposProcedureIncluirFrom(tabelaCampo, visaocampos, queryStringCamposB);
            GeraCamposProcedureIncluirfromJson(tabelaCampo, visaocampos, queryStringCamposC);

            queryString.AppendLine(" SET @CURSOR_" + tabela + " = CURSOR FOR ");
            queryString.AppendLine(" SELECT ");
            queryString.AppendLine(queryStringCamposB.ToString());
            queryString.AppendLine(" FROM ");

            queryString.AppendLine(" OPENJSON ( @JSON ) ");
            queryString.AppendLine(" WITH ( ");
            queryString.AppendLine(queryStringCamposC.Replace("lax ", "").ToString());
            queryString.AppendLine(" ); ");
            queryString.AppendLine(" OPEN @CURSOR_" + tabela + ";");
            CriaSQLFetch(queryString, visaocampos, tabela, tabelaCampo);
            queryString.AppendLine(" WHILE @@FETCH_STATUS = 0 ");
            queryString.AppendLine("    BEGIN ");
            queryString.AppendLine("        IF @" + tabela + "_Id = 0 ");
            queryString.AppendLine("                BEGIN ");
            CriaSQLInsert(queryString, tabela, tabelaCampo);
            queryString.AppendLine("                END ");
            queryString.AppendLine("        ELSE ");
            queryString.AppendLine("                BEGIN ");
            CriaSQLUpdate(queryString, tabela, tabelaCampo);
            queryString.AppendLine("                END ");
            CriaSQLFetch(queryString, visaocampos, tabela, tabelaCampo);
            queryString.AppendLine("    END ");
            queryString.AppendLine(" CLOSE @CURSOR_" + tabela + ";");

            


            queryString.AppendLine("END");

            using (SqlConnection connection =
                new SqlConnection(_stringConecao))
            {
                SqlCommand command = new SqlCommand(queryString.ToString(), connection);

                try
                {
                    connection.Open();
                    var reader = command.ExecuteNonQuery();
                    retorno = (reader == -1);
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }

            return retorno;
        }

        private void GeraCamposProcedureIncluirSelect(List<TabelaCampo> campos, List<VisaoCampo> camposVisao, StringBuilder queryStringCamposA)
        {
            foreach (var itemX in campos)
            {

                var camposVisaoA = camposVisao.Where(p => p.IdTabelaCampo == itemX.Id).FirstOrDefault();

                var geracampo = true;
                if (camposVisaoA != null)
                {
                    geracampo = camposVisaoA.Transferivel;
                }

                if (geracampo == true)
                {
                    if (queryStringCamposA.Length != 0)
                        queryStringCamposA.AppendLine(",");

                    queryStringCamposA.Append("[" + itemX.Campo + "]");
                }

            }
        }

        private void GeraCamposProcedureIncluirFrom(List<TabelaCampo> campos, List<VisaoCampo> camposVisao, StringBuilder queryStringCamposB)
        {
            foreach (var itemX in campos)
            {
                var camposVisaoA = camposVisao.Where(p => p.IdTabelaCampo == itemX.Id).FirstOrDefault();


                if (camposVisaoA == null)
                {
                    if (queryStringCamposB.Length != 0)
                        queryStringCamposB.AppendLine(",");

                    queryStringCamposB.Append("" + itemX.Campo + "");
                }
                if (camposVisaoA != null)
                {
                    var geracampo = true;
                    geracampo = camposVisaoA.Transferivel;
                    if (geracampo == true)
                    {
                        if (string.IsNullOrWhiteSpace(camposVisaoA.PadraoSalva))
                        {
                            if (queryStringCamposB.Length != 0)
                                queryStringCamposB.AppendLine(",");
                            queryStringCamposB.Append("" + itemX.Campo + "");
                        }
                        else
                        {
                            if (queryStringCamposB.Length != 0)
                                queryStringCamposB.AppendLine(",");
                            queryStringCamposB.Append("" + itemX.Campo + " = " + camposVisaoA.PadraoSalva);
                        }
                    }
                }
            }
        }

        private void GeraCamposProcedureIncluirfromJson(List<TabelaCampo> campos, List<VisaoCampo> camposVisao, StringBuilder queryStringCamposC)
        {
            foreach (var itemX in campos)
            {
                var camposVisaoA = camposVisao.Where(p => p.IdTabelaCampo == itemX.Id).FirstOrDefault();
                if (camposVisaoA == null)
                {
                    if (queryStringCamposC.Length != 0)
                        queryStringCamposC.AppendLine(",");

                    queryStringCamposC.Append("" + itemX.Campo + " "
                        + itemX.TipoCampo.TipoBancoDados.Replace("#numpos", itemX.TamanhoCampo).Replace("#numpre", itemX.Precisao.ToString())
                        + " 'lax $."
                        + itemX.Campo + "'");
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(camposVisaoA.PadraoSalva))
                    {
                        var geracampo = true;
                        geracampo = camposVisaoA.Transferivel;
                        if (geracampo == true)
                        {
                            if (queryStringCamposC.Length != 0)
                                queryStringCamposC.AppendLine(",");

                            queryStringCamposC.Append("" + itemX.Campo + " "
                            + itemX.TipoCampo.TipoBancoDados.Replace("#numpos", itemX.TamanhoCampo).Replace("#numpre", itemX.Precisao.ToString())
                            + " 'lax $."
                            + itemX.Campo + "'");
                        }
                    }

                }
            }
        }

        internal string ConsultaDropDown(string sqlQuery)
        {
            string retorno = "";

            if ( string.IsNullOrEmpty(sqlQuery) )
                throw new Exception("Informe a Query");

            if (sqlQuery.ToUpper().Contains("ALTER"))
                throw new Exception("Query Inválida comando ALTER não aceito");
            if (sqlQuery.ToUpper().Contains("PROCEDURE"))
                throw new Exception("Query Inválida comando PROCEDURE não aceito");
            if (sqlQuery.ToUpper().Contains("DROP"))
                throw new Exception("Query Inválida comando DROP não aceito");
            if (sqlQuery.ToUpper().Contains("CREATE"))
                throw new Exception("Query Inválida comando CREATE não aceito");
            if (sqlQuery.ToUpper().Contains("TABLE"))
                throw new Exception("Query Inválida comando TABLE não aceito");
            if (sqlQuery.ToUpper().Contains("TRUNCATE"))
                throw new Exception("Query Inválida comando TRUNCATE não aceito");
            if (sqlQuery.ToUpper().Contains("DELETE"))
                throw new Exception("Query Inválida comando Delete não aceito");
            if (sqlQuery.ToUpper().Contains("UPDATE "))
                throw new Exception("Query Inválida comando Update não aceito");
            if (!sqlQuery.ToUpper().Contains("SELECT"))
                throw new Exception("Query Inválida comando Select não encontrado");
            if (!sqlQuery.ToUpper().Contains("VALUE"))
                throw new Exception("Query Inválida 'Value' não encontrado");
            if (!sqlQuery.ToUpper().Contains("TEXT"))
                throw new Exception("Query Inválida 'Text' não encontrado");

            using (SqlConnection connection =
                         new SqlConnection(_stringConecao))
            {
                SqlCommand command = new SqlCommand(sqlQuery.ToString(), connection);

                try
                {
                    connection.Open();
                    var reader = command.ExecuteReader();
                    reader.Read();
                    retorno = reader[0].ToString();
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }

            return retorno;
        }

        public bool CriarProcedureObter(string atividade, List<AtividadeVisao> atividadeVisao, DBGPControle db)
        {
            // Provide the query string with a parameter placeholder.
            bool retorno = false;
            StringBuilder queryString = new StringBuilder();
            var nomeProcedure = "PRCSF_OBTER_" + atividade;
            DropProcedure(nomeProcedure);
            queryString.AppendLine("CREATE PROCEDURE [dbo].[" + nomeProcedure + "] (@PROCESSOEXECUCAO_ID uniqueidentifier)");
            queryString.AppendLine(" AS");
            queryString.AppendLine(" BEGIN");
            queryString.AppendLine(" SET NOCOUNT ON;");

            queryString.AppendLine(" SELECT");
            


            StringBuilder queryStringQuery = new StringBuilder();

            foreach (var item in atividadeVisao)
            {
                if (queryStringQuery.Length != 0)
                    queryStringQuery.Append(",");

                var nomeTabela = item.Visao.Tabela.Nome;

                queryStringQuery.Append(" ( SELECT * FROM [dbo].[" + nomeTabela + "] where ProcessoExecucao_Id = @PROCESSOEXECUCAO_ID FOR JSON PATH ) AS " + nomeTabela);



            }
            queryString.AppendLine(queryStringQuery.ToString() + ";");
            queryString.AppendLine(" END");

            using (SqlConnection connection =
                new SqlConnection(_stringConecao))
            {
                SqlCommand command = new SqlCommand(queryString.ToString(), connection);

                try
                {
                    connection.Open();
                    var reader = command.ExecuteNonQuery();
                    retorno = (reader == -1);
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }

            return retorno;
        }


        public bool CriarProcedureTabelaObter(string tabela, List<TabelaCampo> tabelaCampo, DBGPControle db)
        {
            // Provide the query string with a parameter placeholder.
            bool retorno = false;
            StringBuilder queryString = new StringBuilder();
            var nomeProcedure = "PRCSF_OBTER_TABELA_" + tabela;
            DropProcedure(nomeProcedure);
            queryString.AppendLine("CREATE PROCEDURE [dbo].[" + nomeProcedure + "] (@REGINI INT, @REGFIM INT)");
            queryString.AppendLine(" AS");
            queryString.AppendLine(" BEGIN");
            queryString.AppendLine(" SET NOCOUNT ON;");

            queryString.AppendLine(" select * FROM ");
            queryString.AppendLine(" (SELECT ROW_NUMBER() OVER ( ORDER BY id ) AS RowNum, * ");
            queryString.AppendLine(" FROM " + tabela + " )  as entidade WHERE   RowNum >= @REGINI AND RowNum < @REGFIM ");
            queryString.AppendLine(" order by Descricao FOR JSON PATH ");


            queryString.AppendLine(" END");

            using (SqlConnection connection =
                new SqlConnection(_stringConecao))
            {
                SqlCommand command = new SqlCommand(queryString.ToString(), connection);

                try
                {
                    connection.Open();
                    var reader = command.ExecuteNonQuery();
                    retorno = (reader == -1);
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }

            return retorno;
        }

    }



}