"Resource/UI/HudMedicCharge.res"
{	
	"Background"
	{
		"ControlName"	"CTFImagePanel"
		"fieldName"		"Background"
		//"xpos"			"-13"
		//"ypos"			"-18"
		"zpos"			"3"
		//"wide"			"112"
		//"tall"			"45"
		wide 375
		tall 85
		xpos -160
		ypos -75
		"visible"		"1"
		"enabled"		"1"
		"image"			"../hud/health_bg"
		"scaleImage"	"1"	
		//"teambg_2"		"../hud/medic_charge_red_bg"
		"teambg_2"			"../hud/health_bg"
		"teambg_3"		"../hud/health_bg"	
			
		
				
	}
	
	 "ChargeLabel"
	{
		"ControlName"	"CTFLabel"
		"fieldName"		"ChargeLabel"
		"xpos"			"0"
		"ypos"			"8"
		"zpos"			"4"
		"wide"			"100"
		"tall"			"15"
		"autoResize"	"0"
		"pinCorner"		"0"
		"visible"		"1"
		"visible_minmode"		"1"
		"enabled"		"1"
		"tabPosition"	"5"
		"labelText"		"#TF_ubercharge"
		"textAlignment"	"west"
		"dulltext"		"0"
		"brighttext"	"0"
		"font"			"ChargeLabelFont"
	}
	
	"ChargeMeter"
	{	
		"ControlName"	"ContinuousProgressBar"
		"fieldName"		"ChargeMeter"
		"font"			"Default"
		"xpos"			"30"
		"xpos_minmode"			"0"
		"ypos"			"38"
		"ypos_minmode"			"0"
		"zpos"			"5"
		"wide"			"95"
		"tall"			"10"				
		"autoResize"		"1"
		"pinCorner"		"1"
		"visible"		"1"
		"enabled"		"1"
		"textAlignment"	"Center"
		"dulltext"		"0"
		"brighttext"	"1"
		"tabPosition"	"5"
		"CornerRadius"	  "5"
		
		
	}		
	
	"HealthClusterIcon"
	{
		"ControlName"	"ImagePanel"
		"fieldName"		"HealthClusterIcon"
		"xpos"			"2"
		"ypos"			"17"
		"wide"			"36"
		"tall"			"36"
		"visible"		"1"
		"visible_minmode"		"0"
		"enabled"		"1"
		"image"			"../hud/ico_health_cluster"
		"scaleImage"	"1"	
	}		

	
}
