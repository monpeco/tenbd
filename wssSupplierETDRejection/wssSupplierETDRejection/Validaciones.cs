using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

/// <summary>
/// Summary description for Validaciones
/// </summary>
public class Validaciones
{
	public Validaciones()
	{
	}

    public static bool validaRut(String tur)
    {
        int rut = 0;
        int Digito;
        int Contador;
        int Multiplo;
        int Acumulador;
        string RutDigito, digito;
        try
        {

            rut = Convert.ToInt32(tur.Substring(0,tur.IndexOf("-")));
            digito = tur.Substring(tur.IndexOf("-") + 1, 1);

            Contador = 2;
            Acumulador = 0;

            while (rut != 0)
            {
                Multiplo = (rut % 10) * Contador;
                Acumulador = Acumulador + Multiplo;
                rut = rut / 10;
                Contador = Contador + 1;
                if (Contador == 8)
                {
                    Contador = 2;
                }

            }

            Digito = 11 - (Acumulador % 11);
            RutDigito = Digito.ToString().Trim();
            if (Digito == 10)
            {
                RutDigito = "K";
            }
            if (Digito == 11)
            {
                RutDigito = "0";
            }

            return (RutDigito == digito);
        }
        catch (Exception ex)
        {
            Console.WriteLine(Convert.ToString(ex));
            return false;
        }
    }
}
