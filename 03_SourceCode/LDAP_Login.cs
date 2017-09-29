using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.DirectoryServices;
using System.DirectoryServices.Protocols;
using System.ComponentModel;
using System.Collections;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            try {
                LdapConnection con = new LdapConnection(new LdapDirectoryIdentifier("addc01.edu.htl-leonding.ac.at:636"), new System.Net.NetworkCredential("in130021" + "@EDU", "Vumnawl4"));
                con.Bind();
                //statt ou students ou teachers
                //Klasse finden
                String[] array = { "dn", "displayName", "gecos" };
                DirectoryRequest dr = new SearchRequest("ou=Students,ou=HTL,DC=EDU,DC=HTL-LEONDING,DC=AC,DC=AT", "(cn=in130021)", System.DirectoryServices.Protocols.SearchScope.Subtree, array);
                var dresp = (System.DirectoryServices.Protocols.SearchResponse)con.SendRequest(dr);
                var entries = dresp.Entries[0].DistinguishedName.Split(',');
                //for teachers
                if(dresp.Entries.Count == 0)
                {
                    DirectoryRequest newDirectoryRequest = new SearchRequest("ou=Teachers,ou=HTL,DC=EDU,DC=HTL-LEONDING,DC=AC,DC=AT", "(cn=in130021)", System.DirectoryServices.Protocols.SearchScope.Subtree, array);
                    var response = (System.DirectoryServices.Protocols.SearchResponse)con.SendRequest(dr);
                    var newEntries = dresp.Entries[0].DistinguishedName.Split(',');
                    //todo get name
                }
                var klasse = entries[1].Split('=')[1];
                var abteilung = entries[2].Split('=')[1];
                var values = dresp.Entries[0].Attributes.Values;
                string name;
                foreach (var item in values)
                {
                    var directoryAttributte = (DirectoryAttribute)item;
                    name = directoryAttributte.GetValues(typeof(string))[0].ToString();
                    break;
                }

                switch (abteilung)
                {
                    case "IF":
                        abteilung = "Informatik";
                        break;
                    case "BG":
                        abteilung = "Medizintechnik";
                        break;
                    case "FE":
                        abteilung = "Fachschule Elektronik";
                        break;
                    case "HE":
                        abteilung = "Elektronik";
                        break;
                    case "IT":
                        abteilung = "Medientechnik";
                        break;
                    case "AD":
                        abteilung = "Abendschule";
                        break;
                    case "KD":
                        abteilung = "Kolleg";
                        break;
                }

                Console.WriteLine("JEJ");
            }
            catch (Exception e)
            {
                Console.Out.WriteLine(e.Message);
            }
            Console.ReadKey();
        }
    }
}
