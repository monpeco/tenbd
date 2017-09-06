using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.IO;
using System.Xml;

namespace WssSupplierETDRejection
{
    public class Parametros
    {
        protected string _codiEmex;
        protected string _ruttEmpr;
        protected string _directorio;
        protected string _egateHome;
        protected string _directorioIn;
        protected string _directorioOut;
        protected string _directorioBin;

        public Parametros()
        {
            this._codiEmex = "";
            this._directorio = "";
            this._egateHome = "";
            this._directorioIn = "";
            this._directorioOut = "";
            this._directorioBin = "";
            this._ruttEmpr = "";
        }

        public Parametros(string rutt_empr)
        {
            this.codiEmex = "";
            this.path = "";
            this.egateHome = "";
            this.pathIn = "";
            this.pathOut = "";
            this.pathBin = "";
            this.ruttEmpr = rutt_empr;
            this.getParametros(rutt_empr);
        }

        public string ruttEmpr
        {
            get
            {
                return this._ruttEmpr;
            }
            set
            {
                this._ruttEmpr = Convert.ToString(value);
            }
        }

        public string pathBin
        {
            get
            {
                return this._directorioBin;
            }
            set
            {
                this._directorioBin = Convert.ToString(value);
            }
        }

        public string pathOut
        {
            get
            {
                return this._directorioOut;
            }
            set
            {
                this._directorioOut = Convert.ToString(value);
            }
        }

        public string pathIn
        {
            get
            {
                return this._directorioIn;
            }
            set
            {
                this._directorioIn = Convert.ToString(value);
            }
        }

        public string egateHome
        {
            get
            {
                return this._egateHome;
            }
            set
            {
                this._egateHome = Convert.ToString(value);
            }
        }

        public string path
        {
            get
            {
                return this._directorio;
            }
            set
            {
                this._directorio = Convert.ToString(value);
            }
        }

        public string codiEmex
        {
            get
            {
                return this._codiEmex;
            }
            set
            {
                this._codiEmex = Convert.ToString(value);
            }
        }


        public void getParametros(string rutt_empr)
        {
            this.ruttEmpr = rutt_empr;
            string rut = "", home = "";
            string baseDir = System.Web.HttpRuntime.AppDomainAppPath;
            string EmpresaPath = Path.Combine(baseDir, "librerias/empresas.xml");
            if (File.Exists(EmpresaPath))
            {
                XmlDocument xDoc = new XmlDocument();
                xDoc.Load(EmpresaPath);
                XmlNodeList empresas = xDoc.GetElementsByTagName("empresas");
                XmlNodeList lista = ((XmlElement)empresas[0]).GetElementsByTagName("empresa");


                foreach (XmlElement nodo in lista)
                {
                    XmlNodeList uno = nodo.GetElementsByTagName("rut");
                    rut = uno[0].InnerText.ToString();
                    if (rut == rutt_empr)
                    {
                        XmlNodeList dos = nodo.GetElementsByTagName("codi_emex");
                        _codiEmex = dos[0].InnerText.ToString();
                        XmlNodeList tres = nodo.GetElementsByTagName("path");
                        _directorio = tres[0].InnerText.ToString();
                        XmlNodeList cuatro = nodo.GetElementsByTagName("home");
                        home = cuatro[0].InnerText.ToString();
                        break;
                    }
                }

            }
            if (home == "" )
            {
                home = "EGATE_HOME";
                _directorio = Convert.ToString(Environment.GetEnvironmentVariable("EGATE_HOME"));
            }

            _egateHome = home;
            _directorioIn = _directorio + @"\in\";
            _directorioOut = _directorio + @"\out\";
            _directorioBin = _directorio + @"\bin\";

        }



    }

    
}
