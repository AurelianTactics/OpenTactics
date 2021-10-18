using System;
//using System.Reflection;
//using System.Text;

//allows for ClassEditObjects for the creating new classes in ClassEditController
//lets user customize classes with the following attributes
[Serializable]
public class ClassEditObject  {

    public int ClassId;
    public int Version;

    public int Icon;
    public int CommandSet;
	public string ClassName;

    public int Move;
    public int Jump;
    public int ClassEvade;

    public int HPBase;
    public int MPBase;
    public int SpeedBase;
    public int PABase;
    public int MABase;
    public int AgiBase;
    public int HPGrowth;
    public int MPGrowth;
    public int SpeedGrowth;
    public int PAGrowth;
    public int MAGrowth;
    public int AgiGrowth;

    public ClassEditObject(int classId, int commandSet, int version)
    {
        this.ClassId = classId;
        this.CommandSet = commandSet;
        this.Version = version;
        this.ClassName = "Custom";
        this.Icon = 1;

        this.Move = 4;
        this.Jump = 3;
        this.ClassEvade = 10;

        if( version == NameAll.VERSION_CLASSIC)
        {
            //mime 140	50	120	120	115	1	6	30	100	35	40	1

            this.HPBase = 140;
            this.MPBase = 50;
            this.SpeedBase = 120;
            this.PABase = 120;
            this.MABase = 115;
            this.AgiBase = 1;
            this.HPGrowth = 6;
            this.MPGrowth = 30;
            this.SpeedGrowth = 100;
            this.PAGrowth = 35;
            this.MAGrowth = 40;
            this.AgiGrowth = 1;
        }
        else
        {
            this.HPBase = 20;
            this.MPBase = 20;
            this.SpeedBase = 6;
            this.PABase = 3;
            this.MABase = 4;
            this.AgiBase = 3;
            this.HPGrowth = 1000;
            this.MPGrowth = 800;
            this.SpeedGrowth = 69;
            this.PAGrowth = 60;
            this.MAGrowth = 60;
            this.AgiGrowth = 60;
        }
	}

	public string GetCEObjectAsString()
	{
		string retString = "";
		retString = " " + this.ClassId + " " + this.CommandSet + " " + this.Version + " " + this.ClassName + " " + this.Icon
			+ " " + this.Move + " " + this.Jump + " " + this.ClassEvade + " " + this.HPBase + " etc ";

		return retString;
	}


	//// this should get all ce info but doesn't for some reason
	//private PropertyInfo[] _PropertyInfos = null;

	//public override string ToString()
	//{
	//	if (_PropertyInfos == null)
	//		_PropertyInfos = this.GetType().GetProperties();

	//	var sb = new StringBuilder();

	//	foreach (var info in _PropertyInfos)
	//	{
	//		var value = info.GetValue(this, null) ?? "(null)";
	//		sb.AppendLine(info.Name + ": " + value.ToString());
	//	}

	//	return sb.ToString();
	//}

}
