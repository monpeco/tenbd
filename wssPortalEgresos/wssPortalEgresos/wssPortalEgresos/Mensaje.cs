using System;

/// <summary>
/// Summary description for Mensaje
/// </summary>
public class Mensaje
{
    protected String sCodigo;
    protected String sMensaje;
    protected String sDTE;
    protected int scantDTE;
    protected string sTipoDoc;
    protected string sFoliDoc;
    protected string sRuttEmis;

    public Mensaje()
    {
        sCodigo = sMensaje = sDTE = sRuttEmis = sTipoDoc = sFoliDoc = "";
        scantDTE = 0;
    }

    public String Codigo
    {
        get { return sCodigo; }
        set { sCodigo = value; }
    }

    public String Mensajes
    {
        get { return sMensaje.TrimEnd(); }
        set { sMensaje = value; }
    }

    public String DTE
    {
        get { return sDTE; }
        set { sDTE = value; }
    }

    public int cantDTE
    {
        get { return scantDTE; }
        set { scantDTE = value; }
    }

    public string TipoDoc
    {
        get { return sTipoDoc; }
        set { sTipoDoc = value; }
    }

    public string FolioDoc
    {
        get { return sFoliDoc; }
        set { sFoliDoc = value; }
    }

    public string RuttEmis
    {
        get { return sRuttEmis; }
        set { sRuttEmis = value; }
    }
}

public class Respuesta
{
    public string SMensaje { get; set; }
    public string SCodigo { get; set; }
}