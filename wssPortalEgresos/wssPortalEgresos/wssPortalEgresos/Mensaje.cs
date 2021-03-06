using System;
using System.Collections.Generic;

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



public class Referencia
{
    public string foliRefe = string.Empty;
    public string tipoRefe = string.Empty;
    public Referencia()
    {
    }
    public Referencia(string foliRefe, string tipoRefe)
    {
        this.foliRefe = foliRefe;
        this.tipoRefe = tipoRefe;
    }
}

public class Documento
{
    public string ruttRece = string.Empty;
    public string digiRece = string.Empty;
    public string ruttEmis = string.Empty;
    public string digiEmis = string.Empty;
    public string tipoDocu = string.Empty;
    public string foliDocu = string.Empty;
    public string fechEmis = string.Empty;
    public string montNeto = string.Empty;
    public string montExen = string.Empty;
    public string montTota = string.Empty;
    //public string corrDocu = string.Empty;
    public string xml = string.Empty;
    public string pdf = string.Empty;
    public List<Referencia> Refencias = new List<Referencia>();
    
    public Documento() { }
    public Documento(string ruttRece, string digiRece, string ruttEmis, string digiEmis, string tipoDocu, string foliDocu, string fechEmis, string montNeto, string montExen, string montTota)
    {
        this.ruttRece = ruttRece;
        this.digiRece = digiRece;
        this.ruttEmis = ruttEmis;
        this.digiEmis = digiEmis;
        this.tipoDocu = tipoDocu;
        this.foliDocu = foliDocu;
        this.fechEmis = fechEmis;
        this.montNeto = montNeto;
        this.montExen = montExen;
        this.montTota = montTota;
    }
    public Documento(string ruttRece, string digiRece, string ruttEmis, string digiEmis, string tipoDocu, string foliDocu, string fechEmis, string montNeto, string montExen, string montTota, string xml, string pdf, List<Referencia> Refencias)
    {
        this.ruttRece = ruttRece;
        this.digiRece = digiRece;
        this.ruttEmis = ruttEmis;
        this.digiEmis = digiEmis;
        this.tipoDocu = tipoDocu;
        this.foliDocu = foliDocu;
        this.fechEmis = fechEmis;
        this.montNeto = montNeto;
        this.montExen = montExen;
        this.montTota = montTota;
        this.xml = xml;
        this.pdf = pdf;
        this.Refencias = Refencias;
    }
}

public class DTEPendietes
{
    public List<Documento> DTE = new List<Documento>();
    public int cantRestantes; 
}