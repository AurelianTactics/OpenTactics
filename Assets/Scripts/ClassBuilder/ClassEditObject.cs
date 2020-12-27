using System;

//allows for ClassEditObjects for the creating new classes in ClassEditController
//lets user customize classes with the following attributes
[Serializable]
public class ClassEditObject  {

    public int ClassId { get; set; }
    public int Version { get; set; }

    public int Icon { get; set; }
    public int CommandSet { get; set; }
    public string ClassName { get; set; }

    public int Move { get; set; }
    public int Jump { get; set; }
    public int ClassEvade { get; set; }

    public int HPBase { get; set; }
    public int MPBase { get; set; }
    public int SpeedBase { get; set; }
    public int PABase { get; set; }
    public int MABase { get; set; }
    public int AgiBase { get; set; }
    public int HPGrowth { get; set; }
    public int MPGrowth { get; set; }
    public int SpeedGrowth { get; set; }
    public int PAGrowth { get; set; }
    public int MAGrowth { get; set; }
    public int AgiGrowth { get; set; }

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


}
