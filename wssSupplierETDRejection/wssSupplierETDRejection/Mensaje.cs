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
/// Summary description for Mensaje
/// </summary>
public class Mensaje
{
    protected String lCodigo;
    protected String lMensaje;

	public Mensaje()
	{
        lCodigo = lMensaje = "";
	}

    public String Codigo
    {
        get
        {
            return lCodigo;
        }
        set
        {
            lCodigo = value;
        }
    }

    public String Descripcion
    {
        get
        {
            return lMensaje;
        }
        set
        {
            lMensaje = value;
        }
    }
}