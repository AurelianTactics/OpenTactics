using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PlayerUnitLevelStats {
    /*
 //given a PlayerUnit object (need the level and the class), modifies the PU's base stats accordingly

 //to do:
    //doesn't seem to be able to get level 1, legacy from old code not sure why
 */
    //10 levels
//    max of the base stats sp/pa/ma/agi overall is 20/25, min is 2
//level range speed is 4 to 14 (more likely 6 to 12)
//lvl range pa/ma/agi is 2 to 15
    public List<int> SetBaseStats(PlayerUnit unit)
    {

        if( unit.ClassId >= NameAll.CUSTOM_CLASS_ID_START_VALUE)
        {
            return SetCustomClassBaseStats(unit);
        }

        List<int> baseStats = new List<int>();
        string classId = unit.ClassId.ToString();
        //Debug.Log("asdf " + Application.dataPath + "\\CSV\\ClassDataCSV.csv");
        string path = Application.dataPath + "/Custom/CSV/ClassDataCSV.csv";
        //Debug.Log(path);
        //var reader = new StreamReader(File.OpenRead(@"Assets\CSV\ClassDataCSV.csv"));
        var reader = new StreamReader(File.OpenRead(path));
        int[] baseValues = new int[16];
        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            var values = line.Split(',');
            if( values[0] == classId)
            {
                for( int i = 0; i < 16; i++)
                {
                    baseValues[i] = Int32.Parse(values[i]); //Debug.Log(" " + baseValues[i]) ;
                }
                break;
            }
        }
        reader.Close(); //could alternatively enclose this in a using block

        //class_id move    jump c_evade hp_base     
        //mp_base speed_base pa_base ma_base agi_base    
        //hp_growth mp_growth   speed_growth pa_growth   ma_growth 
        //agi_growth
        if(unit.ClassId < NameAll.CLASS_FIRE_MAGE) //FFT
        {
            long myLevel = (long)unit.Level;//2;
            long denom = 1638400;

            long rawLife;
            long rawMP;
            long rawSpeed = 98304;
            long rawPA;
            long rawMA;
            long tempLife;
            long tempMP;
            long tempPA;
            long tempMA;
            long tempSpeed;

            int move = baseValues[1];
            int jump = baseValues[2];
            int class_evade = baseValues[3];

            long lifeM = baseValues[4];
            long lifeC = baseValues[10];
            long mpM = baseValues[5];
            long mpC = baseValues[11];
            long speedM = baseValues[6];
            long speedC = baseValues[12];
            long paM = baseValues[7];
            long paC = baseValues[13];
            long maM = baseValues[8];
            long maC = baseValues[14];
            
            if (unit.Sex.Equals("Male"))
            {
                rawPA = 81920;
                rawMA = 65536;
                rawLife = 524287;
                rawMP = 245759;
            }
            else {
                rawPA = 65536;
                rawMA = 81920;
                rawLife = 491519;
                rawMP = 262143;
            }

            if (myLevel == 1)
            {
                myLevel = 2;
            }
            rawSpeed += (rawSpeed / (speedC + 1)) * (myLevel - 1);
            tempSpeed = rawSpeed * speedM / denom;

            rawLife += (rawLife / (lifeC + 1)) * (myLevel - 1);
            tempLife = rawLife * lifeM / denom;

            rawMP += (rawMP / (mpC + 1)) * (myLevel - 1);
            tempMP = rawMP * mpM / denom;

            rawPA += (rawPA / (paC + 1)) * (myLevel - 1);
            tempPA = rawPA * paM / denom;

            rawMA += (rawMA / (maC + 1)) * (myLevel - 1);
            tempMA = rawMA * maM / denom;


            baseStats.Insert(0, (int)tempLife);
            baseStats.Insert(1, (int)tempMP);
            baseStats.Insert(2, (int)tempSpeed);
            baseStats.Insert(3, (int)tempPA);
            baseStats.Insert(4, (int)tempMA);
            baseStats.Insert(5, 1);

            baseStats.Insert(6, move);
            baseStats.Insert(7, jump);
            baseStats.Insert(8, class_evade);
        }
        else
        {
            int level = unit.Level;
            if (level < 1)
            {
                level = 1;
            }
            else if (level > 10)
            {
                level = 10;
            }
            int move = baseValues[1];
            int jump = baseValues[2];
            int class_evade = baseValues[3];

            int tempLife = baseValues[4] + (level * baseValues[10]) / 100;
            int tempMP = baseValues[5] + (level * baseValues[11]) / 100;
            int tempSpeed = baseValues[6] + (level * baseValues[12]) / 100;
            int tempPA = baseValues[7] + (level * baseValues[13]) / 100;
            int tempMA = baseValues[8] + (level * baseValues[14]) / 100;
            int tempAgi = baseValues[9] + (level * baseValues[15]) / 100;

            baseStats.Insert(0, tempLife);
            baseStats.Insert(1, tempMP);
            baseStats.Insert(2, tempSpeed);
            baseStats.Insert(3, tempPA);
            baseStats.Insert(4, tempMA);
            baseStats.Insert(5, tempAgi);

            baseStats.Insert(6, move);
            baseStats.Insert(7, jump);
            baseStats.Insert(8, class_evade);
        }
  
        return baseStats;
    }

    List<int> SetCustomClassBaseStats(PlayerUnit pu)
    {
        ClassEditObject ce = CalcCode.LoadCEObject(pu.ClassId);
        List<int> baseStats = new List<int>();

        if ( ce.Version == NameAll.VERSION_CLASSIC)
        {
            baseStats = GetClassicStats(pu.Level, pu.Sex,ce);
        }
        else
        {
            baseStats = GetAurelianStats(pu.Level, ce);
        }

        return baseStats;
    }

    List<int> GetClassicStats( int level, string sex, ClassEditObject ceObject)
    {
        List<int> baseStats = new List<int>();
        //int[] baseValues = new int[16];

        long myLevel = (long)level;//2;
        long denom = 1638400;

        long rawLife;
        long rawMP;
        long rawSpeed = 98304;
        long rawPA;
        long rawMA;
        long tempLife;
        long tempMP;
        long tempPA;
        long tempMA;
        long tempSpeed;

        int move = ceObject.Move;// baseValues[1];
        int jump = ceObject.Jump;//baseValues[2];
        int class_evade = ceObject.ClassEvade;//baseValues[3];

        long lifeM = ceObject.HPBase;//baseValues[4];
        long lifeC = ceObject.HPGrowth;//baseValues[10];
        long mpM = ceObject.MPBase;//baseValues[5];
        long mpC = ceObject.MPGrowth;//baseValues[11];
        long speedM = ceObject.SpeedBase;//baseValues[6];
        long speedC = ceObject.SpeedGrowth;//baseValues[12];
        long paM = ceObject.PABase;//baseValues[7];
        long paC = ceObject.PAGrowth;//baseValues[13];
        long maM = ceObject.MABase;//baseValues[8];
        long maC = ceObject.MAGrowth;//baseValues[14];

        if (sex == "Male")
        {
            rawPA = 81920;
            rawMA = 65536;
            rawLife = 524287;
            rawMP = 245759;
        }
        else {
            rawPA = 65536;
            rawMA = 81920;
            rawLife = 491519;
            rawMP = 262143;
        }

        if (myLevel == 1)
        {
            myLevel = 2;
        }
        rawSpeed += (rawSpeed / (speedC + 1)) * (myLevel - 1);
        tempSpeed = rawSpeed * speedM / denom;

        rawLife += (rawLife / (lifeC + 1)) * (myLevel - 1);
        tempLife = rawLife * lifeM / denom;

        rawMP += (rawMP / (mpC + 1)) * (myLevel - 1);
        tempMP = rawMP * mpM / denom;

        rawPA += (rawPA / (paC + 1)) * (myLevel - 1);
        tempPA = rawPA * paM / denom;

        rawMA += (rawMA / (maC + 1)) * (myLevel - 1);
        tempMA = rawMA * maM / denom;


        baseStats.Insert(0, Mathf.Clamp((int)tempLife,1,9999));
        baseStats.Insert(1, Mathf.Clamp((int)tempMP,1,999));
        baseStats.Insert(2, Mathf.Clamp((int)tempSpeed,1,99));;
        baseStats.Insert(3, Mathf.Clamp((int)tempPA,1,99));
        baseStats.Insert(4, Mathf.Clamp((int)tempMA,1,99));
        baseStats.Insert(5, 1);

        baseStats.Insert(6, move);
        baseStats.Insert(7, jump);
        baseStats.Insert(8, class_evade);

        return baseStats;
    }

    List<int> GetAurelianStats(int level, ClassEditObject ce)
    {
        List<int> baseStats = new List<int>();
        if (level < 1)
        {
            level = 1;
        }
        else if (level > 10)
        {
            level = 10;
        }
        int move = ce.Move;// baseValues[1];
        int jump = ce.Jump;// baseValues[2];
        int class_evade = ce.ClassEvade;// baseValues[3];

        int tempLife = Mathf.Clamp(ce.HPBase + (level*ce.HPGrowth)/100,1,9999);// baseValues[4] + (level * baseValues[10]) / 100;
        int tempMP = Mathf.Clamp(ce.MPBase + (level * ce.MPGrowth) / 100, 1, 999);// baseValues[5] + (level * baseValues[11]) / 100;
        int tempSpeed = Mathf.Clamp(ce.SpeedBase + (level * ce.SpeedGrowth) / 100,1,20);// baseValues[6] + (level * baseValues[12]) / 100;
        int tempPA = Mathf.Clamp(ce.PABase + (level * ce.PAGrowth) / 100,1,99);// baseValues[7] + (level * baseValues[13]) / 100;
        int tempMA = Mathf.Clamp(ce.MABase + (level * ce.MAGrowth) / 100,1,99);// baseValues[8] + (level * baseValues[14]) / 100;
        int tempAgi = Mathf.Clamp(ce.AgiBase + (level * ce.AgiGrowth) / 100,1,99);// baseValues[9] + (level * baseValues[15]) / 100;

        baseStats.Insert(0, tempLife);
        baseStats.Insert(1, tempMP);
        baseStats.Insert(2, tempSpeed); 
        baseStats.Insert(3, tempPA);
        baseStats.Insert(4, tempMA);
        baseStats.Insert(5, tempAgi);

        baseStats.Insert(6, move);
        baseStats.Insert(7, jump);
        baseStats.Insert(8, class_evade);
        return baseStats;
    }

    public List<int> GetCEBaseStats(ClassEditObject ce, PlayerUnit pu)
    {
        if( ce.Version == NameAll.VERSION_CLASSIC)
        {
            return GetClassicStats(pu.Level, pu.Sex, ce);
        }
        else
        {
            return GetAurelianStats(pu.Level, ce);
        }
    }

    //old FFT way, obsolete
        //classes declared in ClassNames enums in PlayerUnit
        //classes, 1 Chemist, 2 Knight, 3 archer, 4 squire, 5 thief, 6 ninja, 7 monk,
        //8 priest, 9 wizard, 10 time mage, 11 summoner, 12 mediator, 13 oracle,
        //14 geomancer, 15 lancer, 16 samurai, 17 calculator, 18 bard, 19 dancer, 20 mime
    //public List<int> SetPlayerUnitBaseStats(PlayerUnit unit)
    //{
    //    long myLevel = (long)unit.GetLevel();//2;
    //    long denom = 1638400;

    //    long rawLife;
    //    long rawMP;
    //    long rawSpeed = 98304;
    //    long rawPA;
    //    long rawMA;
    //    long tempLife;
    //    long tempMP;
    //    long tempPA;
    //    long tempMA;
    //    long tempSpeed;

    //    //stats that player level uses to calculate base stats
    //    long lifeM = 80;
    //    long lifeC = 12;
    //    long mpM = 75;
    //    long mpC = 16;
    //    long speedM = 100;
    //    long speedC = 100;
    //    long paM = 75;
    //    long paC = 75;
    //    long maM = 80;
    //    long maC = 50;

    //    int move = 3;
    //    int jump = 3;
    //    int class_evade = 5;

    //    //life, mp, speed, pa, ma
    //    List<int> baseStats = new List<int>(); //Debug.Log("in playerlevel stats " + unit.ClassId);

    //    if ( unit.Sex.Equals("Male") )
    //    {
    //        rawPA = 81920;
    //        rawMA = 65536;
    //        rawLife = 524287;
    //        rawMP = 245759;
    //    }
    //    else {
    //        rawPA = 65536;
    //        rawMA = 81920;
    //        rawLife = 491519;
    //        rawMP = 262143;
    //    }

    //    if (unit.ClassId == 1)
    //    {//chemist
    //        lifeM = 80;
    //        lifeC = 12;
    //        mpM = 75;
    //        mpC = 16;
    //        speedM = 100;
    //        speedC = 100;
    //        paM = 75;
    //        paC = 75;
    //        maM = 80;
    //        maC = 50;
    //        move = 3;
    //        jump = 3;
    //        class_evade = 5;
    //    }
    //    else if (unit.ClassId == 2)
    //    {//knight
    //        lifeM = 120;
    //        lifeC = 10;
    //        mpM = 80;
    //        mpC = 15;
    //        speedM = 100;
    //        speedC = 100;
    //        paM = 120;
    //        paC = 40;
    //        maM = 80;
    //        maC = 50;
    //        move = 3;
    //        jump = 3;
    //        class_evade = 10;
    //    }
    //    else if (unit.ClassId == 3)
    //    {//archer
    //        lifeM = 100;
    //        lifeC = 11;
    //        mpM = 65;
    //        mpC = 16;
    //        speedM = 100;
    //        speedC = 100;
    //        paM = 110;
    //        paC = 45;
    //        maM = 80;
    //        maC = 50;
    //        move = 3;
    //        jump = 3;
    //        class_evade = 10;
    //    }
    //    else if (unit.ClassId == 4)
    //    {//squire
    //        lifeM = 100;
    //        lifeC = 11;
    //        mpM = 75;
    //        mpC = 15;
    //        speedM = 100;
    //        speedC = 100;
    //        paM = 90;
    //        paC = 60;
    //        maM = 80;
    //        maC = 50;
    //        move = 4;
    //        jump = 3;
    //        class_evade = 5;
    //    }
    //    else if (unit.ClassId == 5)
    //    {//thief
    //        lifeM = 90;
    //        lifeC = 11;
    //        mpM = 50;
    //        mpC = 16;
    //        speedM = 110;
    //        speedC = 90;
    //        paM = 100;
    //        paC = 50;
    //        maM = 60;
    //        maC = 50;
    //        move = 4;
    //        jump = 4;
    //        class_evade = 25;
    //    }
    //    else if (unit.ClassId == 6)
    //    {//ninja
    //        lifeM = 70;
    //        lifeC = 12;
    //        mpM = 50;
    //        mpC = 13;
    //        speedM = 120;
    //        speedC = 80;
    //        paM = 120;
    //        paC = 43;
    //        maM = 75;
    //        maC = 50;
    //        move = 4;
    //        jump = 4;
    //        class_evade = 30;
    //    }
    //    else if (unit.ClassId == 7)
    //    {//monk
    //        lifeM = 135;
    //        lifeC = 9;
    //        mpM = 80;
    //        mpC = 13;
    //        speedM = 110;
    //        speedC = 100;
    //        paM = 129;
    //        paC = 48;
    //        maM = 80;
    //        maC = 50;
    //        move = 3;
    //        jump = 4;
    //        class_evade = 20;
    //    }
    //    else if (unit.ClassId == 8)
    //    {//priest
    //        lifeM = 80;
    //        lifeC = 10;
    //        mpM = 120;
    //        mpC = 10;
    //        speedM = 110;
    //        speedC = 100;
    //        paM = 90;
    //        paC = 50;
    //        maM = 110;
    //        maC = 50;
    //        move = 3;
    //        jump = 3;
    //        class_evade = 5;
    //    }
    //    else if (unit.ClassId == 9)
    //    {//wizard
    //        lifeM = 75;
    //        lifeC = 12;
    //        mpM = 120;
    //        mpC = 9;
    //        speedM = 100;
    //        speedC = 100;
    //        paM = 60;
    //        paC = 60;
    //        maM = 150;
    //        maC = 50;
    //        move = 3;
    //        jump = 3;
    //        class_evade = 5;
    //    }
    //    else if (unit.ClassId == 10)
    //    {//time mage
    //        lifeM = 75;
    //        lifeC = 12;
    //        mpM = 120;
    //        mpC = 10;
    //        speedM = 100;
    //        speedC = 100;
    //        paM = 50;
    //        paC = 65;
    //        maM = 130;
    //        maC = 50;
    //        move = 3;
    //        jump = 3;
    //        class_evade = 5;
    //    }
    //    else if (unit.ClassId == 11)
    //    {//summoner
    //        lifeM = 70;
    //        lifeC = 13;
    //        mpM = 125;
    //        mpC = 8;
    //        speedM = 90;
    //        speedC = 100;
    //        paM = 50;
    //        paC = 70;
    //        maM = 125;
    //        maC = 50;
    //        move = 3;
    //        jump = 3;
    //        class_evade = 5;
    //    }
    //    else if (unit.ClassId == 12)
    //    {//mediator
    //        lifeM = 80;
    //        lifeC = 11;
    //        mpM = 70;
    //        mpC = 18;
    //        speedM = 100;
    //        speedC = 100;
    //        paM = 75;
    //        paC = 55;
    //        maM = 75;
    //        maC = 50;
    //        move = 3;
    //        jump = 3;
    //        class_evade = 5;
    //    }
    //    else if (unit.ClassId == 13)
    //    {//oracle
    //        lifeM = 75;
    //        lifeC = 12;
    //        mpM = 110;
    //        mpC = 10;
    //        speedM = 100;
    //        speedC = 100;
    //        paM = 50;
    //        paC = 60;
    //        maM = 120;
    //        maC = 50;
    //        move = 3;
    //        jump = 3;
    //        class_evade = 5;
    //    }
    //    else if (unit.ClassId == 14)
    //    {//geomancer
    //        lifeM = 110;
    //        lifeC = 10;
    //        mpM = 95;
    //        mpC = 11;
    //        speedM = 100;
    //        speedC = 100;
    //        paM = 110;
    //        paC = 45;
    //        maM = 105;
    //        maC = 50;
    //        move = 4;
    //        jump = 3;
    //        class_evade = 10;
    //    }
    //    else if (unit.ClassId == 15)
    //    {//lancer
    //        lifeM = 120;
    //        lifeC = 10;
    //        mpM = 50;
    //        mpC = 15;
    //        speedM = 100;
    //        speedC = 100;
    //        paM = 120;
    //        paC = 40;
    //        maM = 50;
    //        maC = 50;
    //        move = 3;
    //        jump = 4;
    //        class_evade = 15;
    //    }
    //    else if (unit.ClassId == 16)
    //    {//samurai
    //        lifeM = 75;
    //        lifeC = 12;
    //        mpM = 75;
    //        mpC = 14;
    //        speedM = 100;
    //        speedC = 100;
    //        paM = 128;
    //        paC = 45;
    //        maM = 90;
    //        maC = 50;
    //        move = 3;
    //        jump = 3;
    //        class_evade = 20;
    //    }
    //    else if (unit.ClassId == 17)
    //    {//calculator
    //        lifeM = 65;
    //        lifeC = 14;
    //        mpM = 80;
    //        mpC = 10;
    //        speedM = 50;
    //        speedC = 100;
    //        paM = 50;
    //        paC = 70;
    //        maM = 70;
    //        maC = 50;
    //        move = 3;
    //        jump = 3;
    //        class_evade = 5;
    //    }
    //    else if (unit.ClassId == 18)
    //    {//bard
    //        lifeM = 55;
    //        lifeC = 20;
    //        mpM = 50;
    //        mpC = 20;
    //        speedM = 100;
    //        speedC = 100;
    //        paM = 30;
    //        paC = 80;
    //        maM = 115;
    //        maC = 50;
    //        move = 3;
    //        jump = 3;
    //        class_evade = 5;
    //    }
    //    else if (unit.ClassId == 19)
    //    {//dancer
    //        lifeM = 60;
    //        lifeC = 20;
    //        mpM = 50;
    //        mpC = 20;
    //        speedM = 100;
    //        speedC = 100;
    //        paM = 110;
    //        paC = 50;
    //        maM = 95;
    //        maC = 50;
    //        move = 3;
    //        jump = 3;
    //        class_evade = 5;
    //    }
    //    else if (unit.ClassId == NameAll.CLASS_MIME)
    //    {//mime
    //        //Debug.Log("in mime shit, redoing mime stats");
    //        lifeM = 140;
    //        lifeC = 6;
    //        mpM = 50;
    //        mpC = 30;
    //        speedM = 120;
    //        speedC = 100;
    //        paM = 120;
    //        paC = 35;
    //        maM = 115;
    //        maC = 40;
    //        move = 4;
    //        jump = 4;
    //        class_evade = 5;
    //    }

    //    if (myLevel == 1)
    //    {
    //        myLevel = 2;
    //    }
    //    rawSpeed += (rawSpeed / (speedC + 1)) * (myLevel - 1);
    //    tempSpeed = rawSpeed * speedM / denom;

    //    rawLife += (rawLife / (lifeC + 1)) * (myLevel - 1);
    //    tempLife = rawLife * lifeM / denom;

    //    rawMP += (rawMP / (mpC + 1)) * (myLevel - 1);
    //    tempMP = rawMP * mpM / denom;

    //    rawPA += (rawPA / (paC + 1)) * (myLevel - 1);
    //    tempPA = rawPA * paM / denom;

    //    rawMA += (rawMA / (maC + 1)) * (myLevel - 1);
    //    tempMA = rawMA * maM / denom;

        
    //    baseStats.Insert(0, (int)tempLife);
    //    baseStats.Insert(1, (int)tempMP);
    //    baseStats.Insert(2, (int)tempSpeed);
    //    baseStats.Insert(3, (int)tempPA);
    //    baseStats.Insert(4, (int)tempMA);

    //    baseStats.Insert(5, move);
    //    baseStats.Insert(6, jump);
    //    baseStats.Insert(7, class_evade);

    //    return baseStats;
    //}
}
