#include <string>
#include <sstream>
#include <fstream>
#include <iostream>
#include <stack>
#include <vector>
#include <map>
using namespace std;
#include <lineaDelimit.h>
#include <lineaDelimit.h>
#include <lineaDetalle.h>
#include <archivoDTE.h>
#include <archivoDteOld.h>
#include <archivoDteBol.h>
#include <split.h>

#include <Log.h>
#include <Directorio.h>
#include <Aplicacion.h>
#include <AppGlobal.h>
#include <SrvPrtSucursal.h>

#include <GrupoArchivoGlobal.h>
#include <parserImpresion.h>
#include <funciones.h>
#include <time.h>

#ifdef FOLICLIESUCU
#include <DteControlDll.h>
#endif

extern Aplicacion *app;

#ifdef __cplusplus
extern "C" {
#endif

void getStr(char s[], char t[], int desde, int hasta)
{
   int  i, j;

   for (j = 0, i = desde - 1; (i < hasta) && (s[i] != '\0'); i++, j++) {
      t[j] = s[i];
   }
   t[j] = '\0';
}

#ifdef FOLICLIESUCU
// Funcion que asigna folio ERP en la sucursal, utilizando la DLL control pos,
// Esta función aplica en el proyecto egateSucursal de egateSrvPrtSucuFoliClie
int asignaFolio(int rutEmis,int tipoDte, int folioOrig,char* pNombreArchivo)
{
	char vMsgLog[256];
	int var=0,vAsignacion=1,vExisteUltimoSalto=0;
	stringstream ss;
	  RptaSolicitaFolio;

	app->getLog()->putLog(3,"Asignando folio legal");
	// Verifica que archivo no asignacion en linea F
	string linea;
	ifstream f1;
	f1.open(pNombreArchivo);
	while(getline(f1, linea)) 
	{
		if(f1.eof())
			vExisteUltimoSalto=0;
		else
			vExisteUltimoSalto=1;

		if(linea[0]=='F')
		{
			app->getLog()->putLog(1,"Ya existia asignacion en el archivo");
			vAsignacion = 0;
		}
	}
	f1.close();

	if(vAsignacion)
	{
		// Solicitando folio de asignacion
		RptaSolicitaFolio=SolicitaFolioRut(tipoDte,rutEmis);//SolicitaFolio(tipoDte);
		if(strcmp(RptaSolicitaFolio.codigo,"OK")==0)
		{
			var = 	RptaSolicitaFolio.folio;

			// Agregando linea F al archivo
			ofstream f2;
			f2.open(pNombreArchivo, ofstream::app);
			if (!vExisteUltimoSalto)
				f2<<"\nF;"<<var<<";"<<endl;
			else
				f2<<"F;"<<var<<";"<<endl;
			f2.close();

			sprintf(vMsgLog,"Folio Legal asignado: %d",var);
			app->getLog()->putLog(1, vMsgLog);

			time_t tiempo = time(0);
			struct tm *tlocal = localtime(&tiempo);
			char output[128];
			strftime(output,128,"%Y-%m-%d",tlocal);

			// Registrando folio en archivo de pareo de asignación
			ss << app->getDirSal()->getDir("out");
			ss <<"folios";
			ss <<getSepDirGlobal()<<"pareo_folio_"<<output<<".txt";

			ofstream f3;
			f3.open(ss.str().c_str(),ofstream::app);

			f3 << rutEmis <<";";
			f3 << tipoDte <<";";
			f3 << folioOrig <<";";
			f3 << var <<";";
			f3 << endl;

			f3.close();
		}
		else
		{
			app->getLog()->putLog(1,"ERROR: No pudo rescatar folio legal");
		}
	}
	return 0;
}
#endif

//Funcion que busca archivos
//y los manda a imprimir
int ImpresionDTESucursal()
{
	char	l_tmp[300];
	int		l_cantImpresiones;
	int		l_cantImpresionesMerito;
	int		l_cont = 0;

	limpiaVrch(l_tmp, strlen(l_tmp));
	app->getLog()->putLog(3,"Rescatando Archivo y numero de impresiones");

	//Rescatando Archivo y numero de impresiones
	strcpy(l_tmp, leeArchivoFlat(&l_cantImpresiones,&l_cantImpresionesMerito));

	if (strlen(l_tmp) == 0) //Si el largo es 0
	{						// no existen mas archivos
		app->getLog()->putLog(1,"No existen (mas) archivos");
		return 0;
	}

	app->getLog()->putLog(2,"Invocando a egateSucursal");

	//Imprime las copias que faltan
	while (l_cont < l_cantImpresiones)
	{
		if (FILE * file = fopen(l_tmp, "r"))
		{
			fclose(file);
			stringstream Cmd(stringstream::in|stringstream::out);

			Cmd << app->getDirEnt()->getDir("bin") << "egateSucursal";
			Cmd << " -h " << app->getPar("-h");
			if (cmpParGlobal("-suc", "1") == 0)
				Cmd << " -suc";
			Cmd << " -tl " << app->getPar("-tl");
			Cmd << " -l " << app->getPar("-l");
			Cmd << " -e " << l_tmp;
			Cmd << " -p";
			Cmd << " -uimp ss_facture";
			if (app->getPar("-rep").length() != 0)
				Cmd << " -rep " << app->getPar("-rep");

			if (app->getPar("-tbol") == "1")
				Cmd << " -tbol ";
			else
				Cmd << " -tf " << app->getPar("-tf");

			app->getLog()->putLog(1,"Ejecutando Comando : " + Cmd.str());
			system(Cmd.str().c_str());//Llamando a comando egateSucursal
			app->getLog()->putLog(1,"Comando Ejecutado");
	    }
		l_cont++;
	}
	//Imprime las copias que faltan Con MÉRITO
	l_cont=0;
	while (l_cont < l_cantImpresionesMerito)
	{
		if (FILE * file = fopen(l_tmp, "r"))
		{
			fclose(file);
			stringstream Cmd(stringstream::in|stringstream::out);

			Cmd << app->getDirEnt()->getDir("bin") << "egateSucursal";
			Cmd << " -h " << app->getPar("-h");
			Cmd << " -merit";
			if (cmpParGlobal("-suc", "1") == 0)
				Cmd << " -suc";
			Cmd << " -tl " << app->getPar("-tl");
			Cmd << " -l " << app->getPar("-l");
			Cmd << " -e " << l_tmp;
			Cmd << " -tf " << app->getPar("-tf");
			Cmd << " -p";
			Cmd << " -uimp ss_facture";

			app->getLog()->putLog(1,"Ejecutando Comando :" + Cmd.str());
			system(Cmd.str().c_str());//Llamando a comando egateSucursal
			app->getLog()->putLog(1,"Comando Ejecutado");
		}
		l_cont++;
	}
	char	nomArch[100];
	char	nombreDTE[100];
	limpiaVrch(nomArch, 100);
	limpiaVrch(nombreDTE, 100);
	eliminaRuta(l_tmp, nomArch);
	strncpy(nombreDTE,nomArch,strlen(nomArch)-4);
	stringstream Cmd(stringstream::in|stringstream::out);

	Cmd << app->getDirEnt()->getDir("bin") << "egateNewMoveTransfer";
	Cmd << " " << nombreDTE;

	app->getLog()->putLog(1,"Ejecutando Comando :" + Cmd.str());
	system(Cmd.str().c_str());//Llamando a comando egateMoveTransfer
	app->getLog()->putLog(1,"Comando Ejecutado");

	return 1;
}

//Setea el archivo que se encontro
//Pero solo los datos que interesan(linea A)
void setArchFlat(char* pNombreArchivo, int* pTipoDte, int* pFolioDte, int* pRuttEmisor)
{
	archivoDTE * archDTE;
	archivoDteExpo * archDTE21;
	archivoDteBol * archBOL;
	app->getLog()->putLog(3,"Revisando Formato de carga");
	try 
	{
		if (app->getPar("-tbol") == "1")
		{
			archBOL = new archivoDteBol(pNombreArchivo, 0);
		}
		else
		{
			if (app->getPar("-tf") == "1")
				archDTE = new archivoDteOld(pNombreArchivo, 0);
			else if (app->getPar("-tf") == "2")
				archDTE = new archivoDTE(pNombreArchivo, 0);
			else
				archDTE21 = new archivoDteExpo(pNombreArchivo, 0);
		}
	}
	catch (int e ) 
	{
		//capturando excepcion
		if (e==0)
		{
	 		string mensaje;
			mensaje = 	"Imposible cargar archivo " + app->getPar("-e");
			app->getLog()->putLog(1,mensaje);
			exit (1);
		}
		app->getLog()->putLog(3,"Carga del archivo realizada");
	}
	if (app->getPar("-tbol") == "1")
	{
		(*pTipoDte) = atoi(archBOL->encabezado->getPosicion(1).c_str());
		(*pFolioDte) = atoi(archBOL->encabezado->getPosicion(3).c_str());
		(*pRuttEmisor) = atoi(archBOL->encabezado->getPosicion(18).c_str());
		delete archBOL;
	}
	else if ((app->getPar("-tf") == "21")||(app->getPar("-tf") == "22")) 
	{
		(*pTipoDte) = atoi(archDTE21->encabezado->getPosicion(1).c_str());
		(*pFolioDte) = atoi(archDTE21->encabezado->getPosicion(3).c_str());
		char rutaExt[20];
		 getStr((char*)archDTE21->encabezado->getPosicion(18).c_str(), rutaExt, 1, strlen(archDTE21->encabezado->getPosicion(18).c_str())-2);
		(*pRuttEmisor) = atoi(rutaExt);
		 archDTE21 = new archivoDteExpo();
	//	 delete archDTE21;
	}
	else 
	{
		(*pTipoDte) = atoi(archDTE->encabezado->getPosicion(1).c_str());
		(*pFolioDte) = atoi(archDTE->encabezado->getPosicion(3).c_str());
		(*pRuttEmisor) = atoi(archDTE->encabezado->getPosicion(18).c_str());
		delete archDTE;
	}	
}

//Busca el archivo a procesar
//lo setea y genera numero de impresiones pendientes
char* leeArchivoFlat(int* pCantImpr, int *pCantImprMerit)
{
	string	l_pathFind;
	string	l_archivo;
	string	mensaje;
	char	l_tmp[300];
	int		l_numeImpr;
	int		l_numeImprMerit;
	int		l_tipoDocto;
	int		l_folioDocto;
	int		l_codiEmpresa;

	//Busca archivos en la ruta especificada
	//archivoProcesar(l_tmp, (char*)l_pathFind.c_str(), "txt");
	if (getArchGlobal2Ext(l_tmp, "txt", "TXT") == 0)
	{
		return "";
	}

	l_archivo = l_tmp;

	mensaje = "Archivo a Procesar " + l_archivo;
	app->getLog()->putLog(3,mensaje);

	app->getLog()->putLog(3,"Rescatando datos del archivo");

	//Setenado archivo para rescatar los datos
	//Tipo DTE, Folio DTE, Rutt Empresa emisora
	setArchFlat((char*)l_archivo.c_str(), &l_tipoDocto, &l_folioDocto, &l_codiEmpresa);

	#ifdef FOLICLIESUCU
	// Asigna Folio ERP desde DLL
	if (getParGlobal("-folioERPSucu"))
	{
		app->getLog()->putLog(3,"Rescatando folio legal");
		asignaFolio(l_codiEmpresa, l_tipoDocto, l_folioDocto, (char*)l_archivo.c_str());
	}
	#endif
	app->getLog()->putLog(3,"Rescatando nro de impresiones");
	//Retorna el numero de impresiones del archivo
	l_numeImpr = retornaNumeImpr(l_codiEmpresa, l_tipoDocto, l_folioDocto);
	l_numeImprMerit= retornaNumeImprMerit(l_codiEmpresa, l_tipoDocto, l_folioDocto);

	mensaje = "Numero impresion pendientes ";
	if (l_numeImpr < 0) //Si pendientes 0
		l_numeImpr = 0;

	mensaje += itoa(l_numeImpr, l_tmp);
	app->getLog()->putLog(3,mensaje);

	mensaje = "Numero impresion pendientes con merito ";
	if (l_numeImprMerit < 0) //Si pendientes 0
		l_numeImprMerit = 0;

	mensaje += itoa(l_numeImprMerit, l_tmp);
	app->getLog()->putLog(3,mensaje);


	*pCantImpr = l_numeImpr;//retornando impresiones pendientes
	*pCantImprMerit = l_numeImprMerit;

	limpiaVrch(l_tmp, strlen(l_tmp));
	strcpy(l_tmp, l_archivo.c_str());

	return l_tmp;//retornando nombre de archivo
}

//Retorna el numero de impresiones pendientes
int retornaNumeImpr(int pCodiEmpresa, int pTipoDocto, int pFolioDocto)
{
	int		l_numeImpr;
	int		l_logImpr;
	int		l_imprPend;
	string	mensaje_log = "";
	char	tmpChar[100];

	mensaje_log = "Rescatando cantidad de impresiones del documento ";
	mensaje_log += itoa(pTipoDocto,tmpChar);
	app->getLog()->putLog(3, mensaje_log);

	//Parsea el nume_impr y devuelve el numero
	//de impresiones para el documento
	l_numeImpr = findNumeImpr(pTipoDocto);

	mensaje_log = "Rescatando log de impresiones del documento ";
	mensaje_log += itoa(pTipoDocto, tmpChar);
	app->getLog()->putLog(3, mensaje_log);

	//Retorna el numero de impresiones del documento
	//parseando el log de impresion del documento
	l_logImpr = findLogImpr(pCodiEmpresa, pTipoDocto, pFolioDocto);

	l_imprPend = l_numeImpr - l_logImpr;

	return l_imprPend;
}


int retornaNumeImprMerit(int pCodiEmpresa, int pTipoDocto, int pFolioDocto)
{
	int		l_numeImprMerit;
	int		l_logImpr;
	int		l_imprPend;
	string	mensaje_log = "";
	char	tmpChar[100];

	mensaje_log = "Rescatando cantidad de impresiones del documento ";
	mensaje_log += itoa(pTipoDocto,tmpChar);
	app->getLog()->putLog(3, mensaje_log);

	//Parsea el nume_impr y devuelve el numero
	//de impresiones para el documento
	l_numeImprMerit = findNumeImprMerit(pTipoDocto);

	mensaje_log = "Rescatando log de impresiones del documento con mérito";
	mensaje_log += itoa(pTipoDocto, tmpChar);
	app->getLog()->putLog(3, mensaje_log);

	//Retorna el numero de impresiones del documento
	//parseando el log de impresion del documento
	l_logImpr = findLogImprMerit(pCodiEmpresa, pTipoDocto, pFolioDocto);

	l_imprPend = l_numeImprMerit - l_logImpr;

	return l_imprPend;
}


#ifdef __cplusplus
}
#endif

