using UnityEngine;
public class Keyword 
{
    public static string SCENE_INGAME = "scene.ingame";
    public static string SCENE_MENU = "scene.menu";

    public static string ANIM_PARAMETER_SWINGSPEED = "swingSpeed";
    public static string ANIM_PARAMETER_ISFLYING = "isFlying";
    public static string ANIM_PARAMETER_ISWALKING = "isWalking";
    public static string ANIM_PARAMETER_ISDISPLAYMISSION = "isDisplayMission";
    public static string ANIM_PARAMETER_MOVEMENTSPEED = "movementSpeed";

    public static int LAYER_DEFAULT = 0;
    public static int LAYER_TERRAIN = 8;
    public static int LAYER_AIRPLANE = 9;
    public static int LAYER_AIRPLANEDAMAGE = 10;
    public static int LAYER_CHARACTER = 11;
    public static int LAYER_LANDINGAREA = 12;
    public static int LAYER_PROPERTY = 13;

    public static string GAMEOBJECT_FOLLOWTARGET = "follow.target";
    public static string GAMEOBJECT_AIRPLANEENTERAREA = "airplane.enter.area";

    public static string TOGGLE_DESCRIPTION_CHARACTERENTER = "Enter Airplane";
    public static string[] TOGGLE_DESCRIPTION_CHARACTERSPACE = { "Super Speed", "Normal Speed" };
    public static string TOGGLE_DESCRIPTION_AIRPLANEENTER = "Exit Airplane";
    public static string[] TOGGLE_DESCRIPTION_AIRPLANESPACE = { "Take Off", "Landing" };

    public static Color COLOR_TOGGLE_CHARACTER = new Color(0.85f, 0.85f, 0f);
    public static Color COLOR_TOGGLE_AIRPLANE = new Color(0F, 0.75F, 0.85F);

    public static string MISSION_1 = "mission.1";
    public static string MISSION_2 = "mission.2";
    public static string MISSION_3 = "mission.3";
    public static string[] MISSION_DESCRIPTION = { "<b><color=yellow>MISI 1</color></b> \nAmbil jamur di hutan", "<color=yellow><b>MISI 2</color></b> " +
            "\n Buat sup jamur di rumah","<color=yellow><b>MISI 3</color></b> \nPergi menuju rumah paman","<color=yellow><b>SEMUA MISI SELESAI</color></b>" };
}
