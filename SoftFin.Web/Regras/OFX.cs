using System.Collections.Generic;
using System.IO;
using System;
using System.Text.RegularExpressions;
using System.Xml;

namespace OFX
{
    public enum TransTypes
    {
        Debit, Credit
    }
    public class Transaction
    {
        public TransTypes TransType;
        public string DatePosted;
        public string TransAmount;
        public string FITID;
        public string Name;
        public string Memo;
        public string CheckNum;
    }
    public class OfxDocument
    {
        public List<Transaction> Transactions;
        public string AccountType;
        public string AccountID;
        public string BankID;
        public string StartDate;
        public string EndDate;
        public string OfxHeader;
        public string Data;
        public string Version;
        public string Security;
        public string Encoding;
        public string Charset;
        public string Compression;
        public string OldFileUID;
        public string NewFileUID;

        public List<Transaction> CarregaOFX(Stream stream) 
        {
            var Transactions = new List<Transaction>();

            using (StreamReader reader = new StreamReader(stream))
            {
                bool inHeader = true;
                while (!reader.EndOfStream)
                {
                    string temp = reader.ReadLine();
                    if (inHeader)
                    {
                        if (temp.ToLower().Contains("<ofx>"))
                        {
                            inHeader = false;
                        }
                        #region Read Header
                        else
                        {
                            string[] tempSplit = temp.Split(":".ToCharArray());
                            switch (tempSplit[0].ToLower())
                            {
                                case "ofxheader":
                                    OfxHeader = tempSplit[1];
                                    break;
                                case "data":
                                    Data = tempSplit[1];
                                    break;
                                case "version":
                                    Version = tempSplit[1];
                                    break;
                                case "security":
                                    Security = tempSplit[1];
                                    break;
                                case "encoding":
                                    Encoding = tempSplit[1];
                                    break;
                                case "charset":
                                    Charset = tempSplit[1];
                                    break;
                                case "compression":
                                    Compression = tempSplit[1];
                                    break;
                                case "oldfileuid":
                                    OldFileUID = tempSplit[1];
                                    break;
                                case "newfileuid":
                                    NewFileUID = tempSplit[1];
                                    break;
                            }
                        }
                        #endregion
                    }
                    if (!inHeader) 
                    {
                        string restOfFile = temp + reader.ReadToEnd();
                        restOfFile = Regex.Replace(restOfFile, Environment.NewLine, "");
                        restOfFile = Regex.Replace(restOfFile, "\n", "");
                        restOfFile = Regex.Replace(restOfFile, "\t", "");
                        BankID = Regex.Match(restOfFile, @"(?<=bankid>).+?(?=<)", RegexOptions.Multiline | RegexOptions.IgnoreCase).Value;
                        AccountID = Regex.Match(restOfFile, @"(?<=acctid>).+?(?=<)", RegexOptions.Multiline | RegexOptions.IgnoreCase).Value;
                        AccountType = Regex.Match(restOfFile, @"(?<=accttype>).+?(?=<)", RegexOptions.Multiline | RegexOptions.IgnoreCase).Value;
                        StartDate = Regex.Match(restOfFile, @"(?<=dtstart>).+?(?=<)", RegexOptions.Multiline | RegexOptions.IgnoreCase).Value;
                        EndDate = Regex.Match(restOfFile, @"(?<=dtend>).+?(?=<)", RegexOptions.Multiline | RegexOptions.IgnoreCase).Value;
                        string banktranlist = Regex.Match(restOfFile, @"(?<=<banktranlist>).+(?=<\/banktranlist>)", RegexOptions.Multiline | RegexOptions.IgnoreCase).Value;

                        MatchCollection m = Regex.Matches(banktranlist, @"<stmttrn>.+?<\/stmttrn>", RegexOptions.Multiline | RegexOptions.IgnoreCase);
                        foreach (Match match in m)
                        {
                            foreach (Capture capture in match.Captures)
                            {
                                Transaction trans = new Transaction();
                                if (Regex.Match(capture.Value, @"(?<=<trntype>).+?(?=<)", RegexOptions.Multiline | RegexOptions.IgnoreCase).Value.ToLower().Equals("credit"))
                                    trans.TransType = TransTypes.Credit;
                                if (Regex.Match(capture.Value, @"(?<=<trntype>).+?(?=<)", RegexOptions.Multiline | RegexOptions.IgnoreCase).Value.ToLower().Equals("debit"))
                                    trans.TransType = TransTypes.Debit;
                                trans.CheckNum = Regex.Match(capture.Value, @"(?<=<checknum>).+?(?=<)", RegexOptions.Multiline | RegexOptions.IgnoreCase).Value;
                                trans.DatePosted = Regex.Match(capture.Value, @"(?<=<dtposted>).+?(?=<)", RegexOptions.Multiline | RegexOptions.IgnoreCase).Value;
                                trans.TransAmount = Regex.Match(capture.Value, @"(?<=<trnamt>).+?(?=<)", RegexOptions.Multiline | RegexOptions.IgnoreCase).Value;
                                trans.FITID = Regex.Match(capture.Value, @"(?<=<fitid>).+?(?=<)", RegexOptions.Multiline | RegexOptions.IgnoreCase).Value;
                                trans.Name = Regex.Match(capture.Value, @"(?<=<name>).+?(?=<)", RegexOptions.Multiline | RegexOptions.IgnoreCase).Value;
                                trans.Memo = Regex.Match(capture.Value, @"(?<=<memo>).+?(?=<)", RegexOptions.Multiline | RegexOptions.IgnoreCase).Value;
                                Transactions.Add(trans);
                            }
                        }
                    }
                }
            }
            return Transactions;
        }
    }

}
