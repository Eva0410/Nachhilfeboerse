using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.DirectoryServices;
using System.DirectoryServices.Protocols;

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
                //String[] array = { "dn", "displayName","gecos" };
                //DirectoryRequest dr = new SearchRequest("ou=Students,ou=HTL,DC=EDU,DC=HTL-LEONDING,DC=AC,DC=AT", "(cn=in130021)", System.DirectoryServices.Protocols.SearchScope.Subtree, array);
                //DirectoryResponse dresp= con.SendRequest(dr);
                //Console.WriteLine(dresp.MatchedDN);
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
