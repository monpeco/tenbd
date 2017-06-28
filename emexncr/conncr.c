
#include <stdio.h>
#include <string.h>
//Include de Clases
#include <AppGlobal.h>
#include <DTEGlobal.h>

//Include de Embebbed
#include <variablesDteC.h>

#include <insertDteBD.h>
#include <funciones.h>
#include <getParam.h>
#include <parserImpresion.h>
#include <logImpresion.h>
#include <traceGlobal.h>
#include <egateSQL.h>
#include <dteSQL.h>

void insertDteBD()
{
	char nomstd[100];
	char pArch[100];
	int pPosicion, pTipoDocu, pCodiEmpr, pFoliDocu, pPosicionAdju;
	int salInsert = 0;
	char pEmail[80+1]="";
	char pEmailcc[80+1]="";
	char pEmailcco[80+1]="";
	char pMailText[2000+1]=""; 
	char pEstaDocu[3+1]="";

	char pTipo[10+1]="";
	char pAdjunto[400+1]="";
	char aux_esta_docu_temp[4];
    char paramNCR[20]="";
    char pCodiEmex[30+1]=""; /****AM|27-06-2017 - OT9359762***/
	limpiaVrch(nomstd,sizeof(nomstd));
	limpiaVrch(pArch,sizeof(pArch));
	limpiaVrch(pEmail,sizeof(pEmail));
	limpiaVrch(pEmailcc,sizeof(pEmailcc));
	limpiaVrch(pMailText,sizeof(pMailText));
	limpiaVrch(pEstaDocu,sizeof(pEstaDocu));
	limpiaVrch(pTipo,sizeof(pTipo));
	limpiaVrch(pAdjunto,sizeof(pAdjunto));
	limpiaVrch(aux_esta_docu_temp,sizeof(aux_esta_docu_temp));
    limpiaVrch(pCodiEmex,sizeof(pCodiEmex)); /****AM|27-06-2017 - OT9359762***/
	strcpy(aux_esta_docu_temp,"PRC");
	
	//Inicio de transaccion
	beginTrans();
	putLogGlobal(1, "Comenzando Insercion en tablas sp");

	//Recatando datos e insertando
	putLogGlobal(2,"Rescatando datos de memoria");
	getLineaAuxiliarGlobal(aux_impr_dest_c, aux_val1_c, aux_val2_c,
		aux_val3_c, aux_val4_c, aux_val5_c,
		aux_val6_c, aux_val7_c, aux_val8_c,
		aux_val9_c);	
	getLineaEncComisionGlobal(&aux_valo_neto_comi_c, &aux_valo_exen_comi_c, &aux_valo_iva_comi_c);
	getEncaGlobal(aux_vers_enca_c, aux_feho_firm_c, &aux_tipo_docu_c,
		&aux_foli_docu_c, aux_fech_emis_c, &aux_indi_nore_c,
		&aux_entr_bien_c, &aux_indi_vegd_c, &aux_vent_serv_c,
		&aux_mont_brut_c, &aux_form_pago_c, aux_fech_canc_c,
		aux_peri_desd_c, aux_peri_hast_c, aux_moda_pago_c,
		aux_codi_tepa_c, &aux_dias_tepa_c, aux_fech_venc_c,
		&aux_rutt_emis_c, aux_digi_emis_c,
		aux_nomb_emis_c, aux_giro_emis_c, aux_nomb_sucu_c,
		&aux_codi_sucu_c, aux_dire_orig_c, aux_comu_orig_c,
		aux_ciud_orig_c, aux_codi_vend_c,
		&aux_rutt_mand_c, aux_digi_mand_c,
		&aux_rutt_rece_c, aux_digi_rece_c, aux_codi_rece_c,
		aux_nomb_rece_c, aux_giro_rece_c, aux_cont_rece_c,
		aux_dire_rece_c, aux_comu_rece_c, aux_ciud_rece_c,
		aux_dire_post_c, aux_comu_post_c, aux_ciud_post_c,
		&aux_rutt_sofa_c, aux_digi_sofa_c,
		aux_info_tran_c, &aux_rutt_tran_c, aux_digi_tran_c,
		aux_dire_dest_c, aux_comu_dest_c, aux_ciud_dest_c,
		&aux_mont_neto_c, &aux_mont_exen_c, &aux_mont_base_c, &aux_tasa_vaag_c,
		&aux_impu_vaag_c,
		&aux_impu_vanr_c, &aux_cred_es65_c, &aux_gara_enva_c,
		&aux_mont_tota_c, &aux_mont_nofa_c, &aux_subt_vese_c,
		&aux_sald_ante_c, &aux_valo_paga_c,
		/*Inicio  Exportacion */
		aux_tipo_impr_c,	&aux_mont_canc_c,	&aux_sald_inso_c,
		&aux_from_paex_c,	&aux_tipo_cupa_c,	&aux_cuen_pago_c,
		aux_banc_pago_c,	aux_glos_pago_c,	&aux_codi_emtr_c,
		&aux_foli_auto_c,	aux_fcau_expo_c,	aux_codi_adic_c,
		aux_iden_adem_c,	aux_iden_reex_c,	aux_naci_ext_c,
		aux_iden_adre_c,	aux_mail_rece_c,	&aux_rutt_chof_c,
		aux_digi_chof_c,	aux_nomb_chof_c,	&aux_moda_vent_c,	
		&aux_clau_expo_c,	&aux_tota_clex_c,	&aux_viaa_tran_c,	
		aux_nomb_tran_c,	&aux_rutt_citr_c,	aux_digi_citr_c,
		aux_nomb_citr_c,	aux_iden_citr_c,	aux_nume_book_c,	
		aux_codi_oper_c,	&aux_codi_puem_c,	aux_iden_puem_c,	
		&aux_codi_pude_c,	aux_iden_pude_c,	&aux_cant_tara_c,	
		&aux_umed_tara_c,	&aux_tota_brut_c,	&aux_unid_brut_c,	
		&aux_tota_neto_c,	&aux_unid_neto_c,	&aux_tota_item_c,	
		&aux_tota_bult_c,	&aux_mont_flet_c,	&aux_mont_segu_c,	
		aux_pais_rece_c,	aux_pais_dest_c,	aux_tipo_mone_c,	
		&aux_mont_baco_c,	&aux_ivag_prop_c,	&aux_ivag_terc_c,	
		aux_tipo_moom_c,	&aux_tipo_camb_c,	&aux_neto_otmo_c,	
		&aux_noaf_otmo_c,	&aux_faca_otmo_c,	&aux_maco_otmo_c,
		&aux_ivag_otmo_c,	&aux_ivno_otmo_c,	&aux_mont_otmo_c
		/*Fin Exportacion */
		);

	if (aux_rutt_emis_c == 0)
	{
		putLogGlobal(1, "El Rut Emisor(empresa) es NULL");
		exit(1);
	}

	putLogGlobal(2, "Rescatando codigo empresa");
	aux_codi_empr_c = getCodigoEmpr(aux_rutt_emis_c);

	//determina si es que el folio debe ser interpretado como folio de ERP (= 1)
	if (getParGlobal("-foliClie"))
	{
		// en (aux_foli_clie_c) queda el folio ERP interno del cliente (ej de SAP)
		limpiaVrch(aux_foli_clie_c, strlen(aux_foli_clie_c));
		r_trim(aux_foli_clie_c, ' ');
		getFolioERPGlobal(aux_foli_clie_c);

		// ahora se recupera y se setea el folio tributario (aux_foli_docu_c)
		aux_foli_docu_c = 0;
		ObtieneFolioERP(aux_codi_empr_c, aux_tipo_docu_c, &aux_foli_docu_c);
		setFolioGlobal(aux_foli_docu_c);
		
		putLogGlobal(2, "Verificando Unicidad de Folio cliente");
		existeFolioClie(aux_codi_empr_c, aux_tipo_docu_c, aux_foli_clie_c);		
	}

	if (getParGlobal("-folioERPSucu"))
	{
		putLogGlobal(1, "folioERPSucu");
		// en (aux_foli_clie_c) queda el folio ERP interno del cliente (ej de SAP)
		limpiaVrch(aux_foli_clie_c, strlen(aux_foli_clie_c));
		r_trim(aux_foli_clie_c, ' ');
		getFolioERPGlobal(aux_foli_clie_c);
	}

	sprintf(aux_tmp, "folio del documento ======= %d", aux_foli_docu_c);
	putLogGlobal(1, aux_tmp);
	sprintf(aux_tmp, "folio de ERP ======= %s", aux_foli_clie_c);
	putLogGlobal(1, aux_tmp);
	
	putLogGlobal(2, "Rescatando numero impresion del documento");
	aux_nume_impr_c = findLogImpr(aux_rutt_emis_c, aux_tipo_docu_c, aux_foli_docu_c);
	//se busca las impresiones con merito.
	putLogGlobal(2, "Rescatando numero impresion del documento con mérito");
	aux_nume_imme_c = findLogImprMerit(aux_rutt_emis_c, aux_tipo_docu_c, aux_foli_docu_c);

	if ((aux_nume_impr_c != 0)||(aux_nume_imme_c != 0))  //log de todo...
	{
		limpiaVrch(usuario_c, strlen(usuario_c));
		limpiaVrch(timestamp_c, strlen(timestamp_c));
		putLogGlobal(2, "Insertando Log");
		parserLogImpresion(aux_rutt_emis_c, aux_tipo_docu_c, aux_foli_docu_c, usuario_c, timestamp_c);
		putLogGlobal(2, "Fin de Insercion Log");
	}

	sprintf(aux_tmp, "Numero de impresiones del documento = %d", aux_nume_impr_c);
	putLogGlobal(2, aux_tmp);
	//Insert Encabezado

	//rendimiento de la aplicación
	traceDte(aux_foli_docu_c,aux_codi_empr_c,aux_tipo_docu_c,"DIN");
	/* aca pregunta por validacion schema a funcion de dteglobal*/
	limpiaVrch(aux_esta_docu_c,4);
	limpiaVrch(aux_msg_sch_c,2001);

	/*Se obtiene estado Inicial*/
	limpiaVrch(pEstaDocu,4);
	ObtieneEstadoInicial(aux_tipo_docu_c, &pEstaDocu);

    /****AM|27-06-2017 - OT9359762***/
    getParamEmpr(paramNCR, aux_codi_empr_c, "DPNR");
    
    getHoldingGlobal(pCodiEmex);
    strcpy(pCodiEmex, "PROD_0000"); //TODO: quitar

    if (strlen(paramNCR)>0)
        ObtieneCriterioNCR(pCodiEmex, aux_codi_empr_c, aux_tipo_docu_c, aux_rutt_rece_c, &pEstaDocu);

    /****AM|27-06-2017 - OT9359762***/

	limpiaVrch(aux_esta_docu_temp_c, sizeof(MAX_ESTA_DOCU + 1));
	
	putLogGlobal(2, "Estado1:");
	putLogGlobal(2, pEstaDocu);
	
	if (strcmp(pEstaDocu,"") == 0)
		strcpy(aux_esta_docu_c,"ING");
	else
		strcpy(aux_esta_docu_c,pEstaDocu);

	strcpy(aux_esta_docu_temp_c,"PRC");
	putLogGlobal(2, "Estado2:");
	putLogGlobal(2, aux_esta_docu_c);
	
	if (getParGlobal("-valsch"))
	{
		/***** Habilitado para generar xml en base de datos ****/
      	validaSchGlobal(aux_esta_docu_c, aux_msg_sch_c, buff); 
		putLogGlobal(2, buff);
	}

    putLogGlobal(2, "Insertando Encabezado");
	putLogGlobal(2, "Estado:");
	putLogGlobal(2, aux_esta_docu_c);
	
	salInsert = insertEncaDocu(aux_codi_empr_c, aux_vers_enca_c, aux_feho_firm_c,
		aux_foli_clie_c,
		aux_tipo_docu_c, aux_foli_docu_c, aux_fech_emis_c,
		aux_indi_nore_c, aux_entr_bien_c, aux_indi_vegd_c,
		aux_vent_serv_c, aux_mont_brut_c, aux_form_pago_c,
		aux_fech_canc_c, aux_peri_desd_c, aux_peri_hast_c,
		aux_moda_pago_c, aux_codi_tepa_c, aux_dias_tepa_c,
		aux_fech_venc_c,
		aux_rutt_emis_c, aux_digi_emis_c,
		aux_nomb_emis_c, aux_giro_emis_c, aux_nomb_sucu_c,
		aux_codi_sucu_c, aux_dire_orig_c, aux_comu_orig_c,
		aux_ciud_orig_c, aux_codi_vend_c,
		aux_rutt_mand_c, aux_digi_mand_c,
		aux_rutt_rece_c, aux_digi_rece_c, aux_codi_rece_c,
		aux_nomb_rece_c, aux_giro_rece_c, aux_cont_rece_c,
		aux_dire_rece_c, aux_comu_rece_c, aux_ciud_rece_c,
		aux_dire_post_c, aux_comu_post_c, aux_ciud_post_c,
		aux_rutt_sofa_c, aux_digi_sofa_c,
		aux_info_tran_c, aux_rutt_tran_c, aux_digi_tran_c,
		aux_dire_dest_c, aux_comu_dest_c, aux_ciud_dest_c,
		aux_mont_neto_c, aux_mont_exen_c, aux_mont_base_c, aux_tasa_vaag_c,
		aux_impu_vaag_c,
		aux_impu_vanr_c, aux_cred_es65_c, aux_gara_enva_c,
		aux_mont_tota_c, aux_mont_nofa_c, aux_subt_vese_c,
		aux_sald_ante_c, aux_valo_paga_c,
		aux_esta_docu_temp_c, aux_msg_sch_c,

		/*Inicio  Exportacion */
		aux_tipo_impr_c,	aux_mont_canc_c,	aux_sald_inso_c,
		aux_from_paex_c,	aux_tipo_cupa_c,	aux_cuen_pago_c,
		aux_banc_pago_c,	aux_glos_pago_c,	aux_codi_emtr_c,
		aux_foli_auto_c,	aux_fcau_expo_c,	aux_codi_adic_c,
		aux_iden_adem_c,	aux_iden_reex_c,	aux_naci_ext_c,
		aux_iden_adre_c,	aux_mail_rece_c,	aux_rutt_chof_c,
		aux_digi_chof_c,	aux_nomb_chof_c,	aux_moda_vent_c,	
		aux_clau_expo_c,	aux_tota_clex_c,	aux_viaa_tran_c,	
		aux_nomb_tran_c,	aux_rutt_citr_c,	aux_digi_citr_c,
		aux_nomb_citr_c,	aux_iden_citr_c,	aux_nume_book_c,	
		aux_codi_oper_c,	aux_codi_puem_c,	aux_iden_puem_c,	
		aux_codi_pude_c,	aux_iden_pude_c,	aux_cant_tara_c,	
		aux_umed_tara_c,	aux_tota_brut_c,	aux_unid_brut_c,	
		aux_tota_neto_c,	aux_unid_neto_c,	aux_tota_item_c,	
		aux_tota_bult_c,	aux_mont_flet_c,	aux_mont_segu_c,	
		aux_pais_rece_c,	aux_pais_dest_c,	aux_tipo_mone_c,	
		aux_mont_baco_c,	aux_ivag_prop_c,	aux_ivag_terc_c,	
		aux_tipo_moom_c,	aux_tipo_camb_c,	aux_neto_otmo_c,	
		aux_noaf_otmo_c,	aux_faca_otmo_c,	aux_maco_otmo_c,	
		aux_ivag_otmo_c,	aux_ivno_otmo_c,	aux_mont_otmo_c,
		/*Fin Exportacion */

		/*j*/
		aux_impr_dest_c, aux_val1_c, aux_val2_c,
		aux_val3_c, aux_val4_c, aux_val5_c,
		aux_val6_c, aux_val7_c, aux_val8_c,
		aux_val9_c
		/*j*/,
		aux_nume_impr_c, aux_nume_imme_c);  //se agregan impresiones de merito

	if (salInsert == 1)
	{
		putLogGlobal(2, "Actualizando comisiones encabezado");		
		comision_encabezado(aux_tipo_docu_c, aux_foli_docu_c, aux_codi_empr_c, 
						 aux_valo_neto_comi_c, aux_valo_exen_comi_c, aux_valo_iva_comi_c);

		putLogGlobal(2, "Insertando en Tabla AccionesDTE");
		iAccionesDTEBeginGlobal();
		while (getAccionesDteGlobal(aux_codi_aced_c,&aux_nuim_soli_c,aux_logo_publ_c,aux_plan_docu_c) != 0)
		{
			//Inserta Acciones
			insertAccionesDTE(aux_codi_empr_c, aux_tipo_docu_c, aux_foli_docu_c,
							aux_codi_aced_c, aux_nuim_soli_c, aux_logo_publ_c,aux_plan_docu_c);
		}

		/*MANEJO CONDICIONAL DE GRABACION DE XML EN LA BD*/
		if (getParGlobal("-xmlbd"))
		{		
			/*genera xml*/
			if (strcmp(aux_esta_docu_c,"ING")==0)
			{
				DTEPutBD(aux_codi_empr_c, aux_tipo_docu_c, aux_foli_docu_c, (unsigned char *)"XML", buff );
			}
		}

		if (getParGlobal("-norbd"))
		{
			putLogGlobal(2, "Insertando Actividades Economicas");
			iAcecBeginGlobal();
			while (getAcecGlobal(&aux_corr_acec_c, &aux_codi_acec_c) != 0)
			{
				//Insert Actividad(es) Economica(s)
				insertDetaAcec(aux_codi_empr_c, aux_tipo_docu_c, aux_foli_docu_c,
					aux_corr_acec_c, aux_codi_acec_c);
			}

			putLogGlobal(2, "Insertando Imp. y Reten.");
			iImptoRetenBeginGlobal();
			while (getImptoRetenGlobal(&aux_corr_imre_c, &aux_codi_impu_c, &aux_tasa_impu_c, &aux_mont_impu_c) != 0)
			{
				//Insert Impuestos Retenciones
				insertSumaImpu(aux_codi_empr_c, aux_tipo_docu_c, aux_foli_docu_c,
					aux_codi_impu_c, aux_tasa_impu_c, aux_mont_impu_c);
			}
			putLogGlobal(2, "Encabezado Insertado");

			//traza despues de insertar el encabezado
			traceDte(aux_foli_docu_c,aux_codi_empr_c,aux_tipo_docu_c,"DEN");

			
			/*inicio Exportaciones*/
			putLogGlobal(2, "Insertando Tipo Bultos");
			iTpoBultoBeginGlobal();
			while (getTpoBultoGlobal(&aux_corr_bult_c, &aux_codi_tibu_c,  &aux_cant_bult_c,  
									 aux_iden_marc_c,aux_iden_cont_c,  aux_sello_cont_c, 
									 aux_nomb_emis_c) != 0)
			{
				//Insert Tipo Bultos
				insertTpoBult(aux_codi_empr_c,  aux_tipo_docu_c,  aux_foli_docu_c,
							   aux_codi_tibu_c,  aux_cant_bult_c,  aux_iden_marc_c,
							   aux_iden_cont_c,  aux_sello_cont_c, aux_nomb_emis_c);
			}

			putLogGlobal(2, "Insertando Suma Imom");
			iImpRetOtrMndaBeginGlobal();
			while (getImpRetOtrMndaGlobal(&aux_corr_imom_c, aux_codi_imom_c,  
										  &aux_valo_imre_c,  &aux_tasa_imom_c) != 0)
			{
				//Insert Suma Imom ojo
				insertSumaImom(aux_codi_empr_c,  aux_tipo_docu_c,  aux_foli_docu_c,
							   aux_codi_imom_c,  aux_valo_imre_c,  aux_tasa_imom_c);

			}
			/*fin Exportaciones*/	
			
			iDetalleBeginGlobal();
			while (getDetalleGlobal(&aux_nume_line_c, aux_nomb_item_c, aux_desc_item_c,
				&aux_indi_exen_c, &aux_cant_refe_c, aux_unid_refe_c,
				&aux_prec_refe_c, &aux_cant_item_c, aux_fech_elab_c,
				aux_fech_vepr_c, aux_unid_medi_c, &aux_prec_item_c,
				&aux_desc_porc_c, &aux_dcto_item_c, &aux_reca_porc_c,
				&aux_reca_item_c, aux_codi_impa_c, &aux_neto_item_c,
				aux_codi_mone_c, &aux_fact_conv_c, &aux_prec_mono_c,
				&aux_tpo_doc_liq_c,
				&aux_desc_mone_c, &aux_reca_mone_c, &aux_valo_mone_c,
				aux_indi_agen_c, &aux_base_faen_c, &aux_marg_comer_c, 
				&aux_prne_cofi_c)!= 0)
			{
				//Insert Detalle
				putLogGlobal(2, "Insertando Detalle");
				insertDetaPrse(aux_codi_empr_c, aux_tipo_docu_c, aux_foli_docu_c,
					aux_nume_line_c, aux_nomb_item_c, aux_desc_item_c,
					aux_indi_exen_c, aux_cant_refe_c, aux_unid_refe_c,
					aux_prec_refe_c, aux_cant_item_c, aux_fech_elab_c,
					aux_fech_vepr_c, aux_unid_medi_c, aux_prec_item_c,
					aux_desc_porc_c, aux_dcto_item_c, aux_reca_porc_c,
					aux_reca_item_c, aux_codi_impa_c, aux_neto_item_c,
					aux_codi_mone_c, aux_fact_conv_c, aux_prec_mono_c,
				
					/*Exportacion*/
					aux_desc_mone_c, aux_reca_mone_c, aux_valo_mone_c,
					aux_indi_agen_c, aux_base_faen_c, aux_marg_comer_c, 
					aux_prne_cofi_c
					/*Exportacion*/
					);
				
				if(aux_tpo_doc_liq_c > 0){
				putLogGlobal(2, "Actualizando Tipo Doc Liq Detalle");
				//getTpoDocLiqDetGlobal(aux_nume_line_c, &aux_tpo_doc_liq_c);

				tpo_doc_liq_deta(aux_tipo_docu_c, aux_foli_docu_c, aux_codi_empr_c, 
							 aux_nume_line_c, aux_tpo_doc_liq_c);
				}
				
				putLogGlobal(2, "Insertando Codigos de Impuestos");

				iCdgItemBeginGlobal(aux_nume_line_c);
				while (getCdgItemGlobal(aux_nume_line_c, &aux_corr_codi_c,
					aux_tipo_codi_c, aux_codi_item_c) != 0)
				{
					//Insert CodigoItem
					insertDetaCodi(aux_codi_empr_c, aux_tipo_docu_c, aux_foli_docu_c,
						aux_nume_line_c, aux_corr_codi_c, aux_tipo_codi_c,
						aux_codi_item_c);
				}

				putLogGlobal(2, "Insertando Subcantidad");
				iSubcantidadBeginGlobal(aux_nume_line_c);
				while (getSubcantidadGlobal(aux_nume_line_c, &aux_corr_suca_c,
					aux_codi_suca_c, &aux_suca_dist_c,aux_tipo_suca_c) != 0)
				{
					//Insert SubCantidad
					insertSucaItem(aux_codi_empr_c, aux_tipo_docu_c, aux_foli_docu_c,
						aux_nume_line_c, aux_corr_suca_c, aux_codi_suca_c,
						aux_suca_dist_c,aux_tipo_suca_c);
				}

				putLogGlobal(2, "Insertando Subdescuentos");
				iSubDsctoBeginGlobal(aux_nume_line_c);
				while (getSubDsctoGlobal(aux_nume_line_c, &aux_corr_deit_c,
					aux_tipo_desc_c, &aux_valo_desc_c) != 0)
				{
					//Insert SubDescuentos
					insertDescItem(aux_codi_empr_c, aux_tipo_docu_c, aux_foli_docu_c,
						aux_nume_line_c, aux_corr_deit_c, aux_tipo_desc_c,
						aux_valo_desc_c);
				}

				putLogGlobal(2, "Insertando SubRecargo");
				iSubRecargoBeginGlobal(aux_nume_line_c);
				while (getSubRecargoGlobal(aux_nume_line_c, &aux_corr_reit_c,
					aux_tipo_reca_c, &aux_valo_reca_c) != 0)
				{
					//Insert SubRecargo
					insertRecaItem(aux_codi_empr_c, aux_tipo_docu_c, aux_foli_docu_c,
						aux_nume_line_c, aux_corr_reit_c, aux_tipo_reca_c,
						aux_valo_reca_c);
				}
			}
			putLogGlobal(2, "Detalle Insertado");

			//trazando la inserción de los detalles
			traceDte(aux_foli_docu_c,aux_codi_empr_c,aux_tipo_docu_c,"DDE");

			putLogGlobal(2, "Insertando Desc. y Recargos");
			iDscRcgGlBeginGlobal();
			while (getDscRcgGlGlobal(&aux_nume_dere_c, aux_tipo_dere_c, &aux_indi_exen_c,
				aux_glos_dere_c, aux_tipo_valo_c, &aux_valo_dere_c,&aux_vomo_dere_c) != 0)
			{
				//Insert DescuentosRecargosGlobales
				insertDescReca(aux_codi_empr_c, aux_tipo_docu_c, aux_foli_docu_c,
					aux_nume_dere_c, aux_tipo_dere_c, aux_indi_exen_c,
					aux_glos_dere_c, aux_tipo_valo_c, aux_valo_dere_c,aux_vomo_dere_c);
			}

			putLogGlobal(2, "Insertando Referencias");
			iReferenciaBeginGlobal();
			while (getReferenciaGlobal(&aux_nume_refe_c, aux_tipo_refe_c, &aux_indi_regl_c,
				aux_foli_refe_c, &aux_rutt_otro_c, aux_digi_otro_c,
				aux_fech_refe_c, aux_codi_refe_c, aux_razo_refe_c) != 0)
			{
				//Inserta Referencia
				int resp=0;

				resp = insertDocuRefe(aux_codi_empr_c, aux_tipo_docu_c, aux_foli_docu_c,
					aux_nume_refe_c, aux_tipo_refe_c, aux_indi_regl_c,
					aux_foli_refe_c, aux_rutt_otro_c, aux_digi_otro_c,
					aux_fech_refe_c, aux_codi_refe_c, aux_razo_refe_c);

				if (resp!=1)
				{
					if(resp == 2)
					{
						strcpy(aux_esta_docu_c,"REV");
					}
					else
					{
						putLogGlobal(2," Error Insertando Referencias");	
						return 0;
					}

				}

			}
			
			putLogGlobal(2, "Insertando Comisiones y Otros Cargos");
			iComisionBeginGlobal();
			while (getComisionGlobal(&aux_nume_comi_c, aux_tipo_movi_comi_c, aux_glos_comi_c, 
						  &aux_tasa_comi_c,&aux_valo_neto_comi_c, &aux_valo_exen_comi_c, 
						  &aux_valo_iva_comi_c) != 0)
			{
				//Inserta Comision
				putLogGlobal(2, "En el Ciclo Insertando Comisiones y Otros Cargos");
				insertDocuComi(aux_codi_empr_c, aux_tipo_docu_c, aux_foli_docu_c,
						aux_nume_comi_c, aux_tipo_movi_comi_c, aux_glos_comi_c,
						aux_tasa_comi_c, aux_valo_neto_comi_c, aux_valo_exen_comi_c,
						aux_valo_iva_comi_c);
			}
			
		
		}
		
		// Insertar en Dte_Envi_PDF
		iemailDTEBegin();
		while (getEmailGlobal(&pPosicion,pEmail,pEmailcc,pEmailcco,pMailText)!= 0) 
		{
			putLogGlobal(2, "En el Ciclo Insertando Mail en Dte_Envi_PDF ");
			insertDteEnviPdf(aux_codi_empr_c,aux_tipo_docu_c, aux_foli_docu_c,
					pPosicion,  pEmail,  pEmailcc, pEmailcco, pMailText);	
		}

 		//Insert de archivos adjuntos
		putLogGlobal(1, "Insertando Adjunto");
		iadjuntoDTEBegin();
		putLogGlobal(1, "Insertando Adjunto2");
		while (getAdjuntoGlobal(&pPosicionAdju,pTipo,pAdjunto)!= 0)	 
		{
			putLogGlobal(2, "En el Ciclo Insertando Arch. Adjunto en DTE_ARCH_ADJU");
			insertDteEnviAdju(aux_codi_empr_c,aux_tipo_docu_c, aux_foli_docu_c,
					pPosicionAdju,  pTipo,  pAdjunto);	
		}
		
		putLogGlobal(1, "Insercion Terminada en tablas sp");
		//trasando el programa despues de todas las inserciones en la base de datos
		traceDte(aux_foli_docu_c,aux_codi_empr_c,aux_tipo_docu_c,"DUI");
		putLogGlobal(1, "Despues de traceDte");

		// si se esta trabajando con folios de cliente
		if (getParGlobal("-foliClie") || getParGlobal("-folioERPSucu"))
		{
			putLogGlobal(1, "Dentro if foliClie");
			insertFoliClie(aux_codi_empr_c, aux_tipo_docu_c, aux_foli_docu_c,
				aux_foli_clie_c, 0);
		}

		putLogGlobal(1, "Antes de commit");

		updateEstaDocuIns(aux_codi_empr_c,aux_tipo_docu_c,aux_foli_docu_c,aux_esta_docu_c);
		commitBD();
		/* si realizo el commit pasa el archivo a proc_ok */
		putLogGlobal(1, "Antes de getNomDte");
		getNomDte(nomstd); /* funcion de dteGlobal*/
		putLogGlobal(1, "Antes de getParGlobalStr");
		getParGlobalStr("-e", pArch);
		putLogGlobal(1, "Antes de monitor_mover_archivo");
		monitor_mover_archivo(pArch, nomstd, 1);
		putLogGlobal(1, "fin..");
	}
	else
	{
		putLogGlobal(1, "No existe ted y archivo es de Transferencia");
		getParGlobalStr("-e", pArch);
		putLogGlobal(1, "Se deja en misma ubicacion archivo: ");
		putLogGlobal(1, pArch);
		exit(1);
	}
}
