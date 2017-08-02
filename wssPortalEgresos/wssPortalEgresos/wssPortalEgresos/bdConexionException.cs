using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Runtime.Serialization;

namespace conexionBaseDatos
{
    public class bdConexionException : Exception
    {

        public bdConexionException() { }

        public bdConexionException(string message) : base(message) { }

        public bdConexionException(string message, Exception inner) : base(message) { }

        protected bdConexionException(SerializationInfo info, StreamingContext context) { }

    }
}