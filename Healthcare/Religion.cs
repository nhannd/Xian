using System;
using System.Collections;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare
{

    /// <summary>
    /// Religion enumeration
    /// </summary>
    public enum Religion
    {
        /// <summary> 
        /// Unknown
        /// </summary>
        [EnumValue("Unknown")]
        VAR,

        /// <summary> 
        /// Agnostic
        /// </summary>
        [EnumValue("Agnostic")]
        AGN,

        /// <summary> 
        /// Atheist
        /// </summary>
        [EnumValue("Atheist")]
        ATH,

        /// <summary> 
        /// Baha'i
        /// </summary>
        [EnumValue("Baha'i")]
        BAH,

        /// <summary> 
        /// Buddhist: Mahayana
        /// </summary>
        [EnumValue("Buddhist: Mahayana")]
        BMA,

        /// <summary> 
        /// Buddhist
        /// </summary>
        [EnumValue("Buddhist")]
        BUD,

        /// <summary> 
        /// Buddhist: Theravada
        /// </summary>
        [EnumValue("Buddhist: Theravada")]
        BTH,

        /// <summary> 
        /// Buddhist: Tantrayana
        /// </summary>
        [EnumValue("Buddhist: Tantrayana")]
        BTA,

        /// <summary> 
        /// Buddhist: Other
        /// </summary>
        [EnumValue("Buddhist: Other")]
        BOT,

        /// <summary> 
        /// Chinese Folk Religionist
        /// </summary>
        [EnumValue("Chinese Folk Religionist")]
        CFR,

        /// <summary> 
        /// Christian
        /// </summary>
        [EnumValue("Christian")]
        CHR,

        /// <summary> 
        /// Christian: American Baptist Church
        /// </summary>
        [EnumValue("Christian: American Baptist Church")]
        ABC,

        /// <summary> 
        /// Christian: African Methodist Episcopal
        /// </summary>
        [EnumValue("Christian: African Methodist Episcopal")]
        AMT,

        /// <summary> 
        /// Christian: African Methodist Episcopal Zion
        /// </summary>
        [EnumValue("Christian: African Methodist Episcopal Zion")]
        AME,

        /// <summary> 
        /// Christian: Anglican
        /// </summary>
        [EnumValue("Christian: Anglican")]
        ANG,

        /// <summary> 
        /// Christian: Assembly of God
        /// </summary>
        [EnumValue("Christian: Assembly of God")]
        AOG,

        /// <summary> 
        /// Christian: Baptist
        /// </summary>
        [EnumValue("Christian: Baptist")]
        BAP,

        /// <summary> 
        /// Christian: Roman Catholic
        /// </summary>
        [EnumValue("Christian: Roman Catholic")]
        CAT,

        /// <summary> 
        /// Christian: Christian Reformed
        /// </summary>
        [EnumValue("Christian: Christian Reformed")]
        CRR,

        /// <summary> 
        /// Christian: Christian Science
        /// </summary>
        [EnumValue("Christian: Christian Science")]
        CHS,

        /// <summary> 
        /// Christian: Christian Missionary Alliance
        /// </summary>
        [EnumValue("Christian: Christian Missionary Alliance")]
        CMA,

        /// <summary> 
        /// Christian: Church of Christ
        /// </summary>
        [EnumValue("Christian: Church of Christ")]
        COC,

        /// <summary> 
        /// Christian: Church of God
        /// </summary>
        [EnumValue("Christian: Church of God")]
        COG,

        /// <summary> 
        /// Christian: Church of God in Christ
        /// </summary>
        [EnumValue("Christian: Church of God in Christ")]
        COI,

        /// <summary> 
        /// Christian: Community
        /// </summary>
        [EnumValue("Christian: Community")]
        COM,

        /// <summary> 
        /// Christian: Congregational
        /// </summary>
        [EnumValue("Christian: Congregational")]
        COL,

        /// <summary> 
        /// Christian: Eastern Orthodox
        /// </summary>
        [EnumValue("Christian: Eastern Orthodox")]
        EOT,

        /// <summary> 
        /// Christian: Evangelical Church
        /// </summary>
        [EnumValue("Christian: Evangelical Church")]
        EVC,

        /// <summary> 
        /// Christian: Episcopalian
        /// </summary>
        [EnumValue("Christian: Episcopalian")]
        EPI,

        /// <summary> 
        /// Christian: Free Will Baptist
        /// </summary>
        [EnumValue("Christian: Free Will Baptist")]
        FWB,

        /// <summary> 
        /// Christian: Friends
        /// </summary>
        [EnumValue("Christian: Friends")]
        FRQ,

        /// <summary> 
        /// Christian: Greek Orthodox
        /// </summary>
        [EnumValue("Christian: Greek Orthodox")]
        GRE,

        /// <summary> 
        /// Christian: Jehovah's Witness
        /// </summary>
        [EnumValue("Christian: Jehovah's Witness")]
        JWN,

        /// <summary> 
        /// Christian: Lutheran
        /// </summary>
        [EnumValue("Christian: Lutheran")]
        LUT,

        /// <summary> 
        /// Christian: Lutheran Missouri Synod
        /// </summary>
        [EnumValue("Christian: Lutheran Missouri Synod")]
        LMS,

        /// <summary> 
        /// Christian: Mennonite
        /// </summary>
        [EnumValue("Christian: Mennonite")]
        MEN,

        /// <summary> 
        /// Christian: Methodist
        /// </summary>
        [EnumValue("Christian: Methodist")]
        MET,

        /// <summary> 
        /// Christian: Latter-day Saints
        /// </summary>
        [EnumValue("Christian: Latter-day Saints")]
        MOM,

        /// <summary> 
        /// Christian: Church of the Nazarene
        /// </summary>
        [EnumValue("Christian: Church of the Nazarene")]
        NAZ,

        /// <summary> 
        /// Christian: Orthodox
        /// </summary>
        [EnumValue("Christian: Orthodox")]
        ORT,

        /// <summary> 
        /// Christian: Other
        /// </summary>
        [EnumValue("Christian: Other")]
        COT,

        /// <summary> 
        /// Christian: Other Protestant
        /// </summary>
        [EnumValue("Christian: Other Protestant")]
        PRC,

        /// <summary> 
        /// Christian: Pentecostal
        /// </summary>
        [EnumValue("Christian: Pentecostal")]
        PEN,

        /// <summary> 
        /// Christian: Other Pentecostal
        /// </summary>
        [EnumValue("Christian: Other Pentecostal")]
        COP,

        /// <summary> 
        /// Christian: Presbyterian
        /// </summary>
        [EnumValue("Christian: Presbyterian")]
        PRE,

        /// <summary> 
        /// Christian: Protestant
        /// </summary>
        [EnumValue("Christian: Protestant")]
        PRO,

        /// <summary> 
        /// Christian: Friends
        /// </summary>
        [EnumValue("Christian: Friends")]
        QUA,

        /// <summary> 
        /// Christian: Reformed Church
        /// </summary>
        [EnumValue("Christian: Reformed Church")]
        REC,

        /// <summary> 
        /// Christian: Reorganized Church of Jesus Christ-LDS
        /// </summary>
        [EnumValue("Christian: Reorganized Church of Jesus Christ-LDS")]
        REO,

        /// <summary> 
        /// Christian: Salvation Army
        /// </summary>
        [EnumValue("Christian: Salvation Army")]
        SAA,

        /// <summary> 
        /// Christian: Seventh Day Adventist
        /// </summary>
        [EnumValue("Christian: Seventh Day Adventist")]
        SEV,

        /// <summary> 
        /// Christian: Southern Baptist
        /// </summary>
        [EnumValue("Christian: Southern Baptist")]
        SOU,

        /// <summary> 
        /// Christian: United Church of Christ
        /// </summary>
        [EnumValue("Christian: United Church of Christ")]
        UCC,

        /// <summary> 
        /// Christian: United Methodist
        /// </summary>
        [EnumValue("Christian: United Methodist")]
        UMD,

        /// <summary> 
        /// Christian: Unitarian
        /// </summary>
        [EnumValue("Christian: Unitarian")]
        UNI,

        /// <summary> 
        /// Christian: Unitarian Universalist
        /// </summary>
        [EnumValue("Christian: Unitarian Universalist")]
        UNU,

        /// <summary> 
        /// Christian: Wesleyan
        /// </summary>
        [EnumValue("Christian: Wesleyan")]
        WES,

        /// <summary> 
        /// Christian: Wesleyan Methodist
        /// </summary>
        [EnumValue("Christian: Wesleyan Methodist")]
        WMC,

        /// <summary> 
        /// Confucian
        /// </summary>
        [EnumValue("Confucian")]
        CNF,

        /// <summary> 
        /// Ethnic Religionist
        /// </summary>
        [EnumValue("Ethnic Religionist")]
        ERL,

        /// <summary> 
        /// Hindu
        /// </summary>
        [EnumValue("Hindu")]
        HIN,

        /// <summary> 
        /// Hindu: Vaishnavites
        /// </summary>
        [EnumValue("Hindu: Vaishnavites")]
        HVA,

        /// <summary> 
        /// Hindu: Shaivites
        /// </summary>
        [EnumValue("Hindu: Shaivites")]
        HSH,

        /// <summary> 
        /// Hindu: Other
        /// </summary>
        [EnumValue("Hindu: Other")]
        HOT,

        /// <summary> 
        /// Jain
        /// </summary>
        [EnumValue("Jain")]
        JAI,

        /// <summary> 
        /// Jewish
        /// </summary>
        [EnumValue("Jewish")]
        JEW,

        /// <summary> 
        /// Jewish: Conservative
        /// </summary>
        [EnumValue("Jewish: Conservative")]
        JCO,

        /// <summary> 
        /// Jewish: Orthodox
        /// </summary>
        [EnumValue("Jewish: Orthodox")]
        JOR,

        /// <summary> 
        /// Jewish: Other
        /// </summary>
        [EnumValue("Jewish: Other")]
        JOT,

        /// <summary> 
        /// Jewish: Reconstructionist
        /// </summary>
        [EnumValue("Jewish: Reconstructionist")]
        JRC,

        /// <summary> 
        /// Jewish: Reform
        /// </summary>
        [EnumValue("Jewish: Reform")]
        JRF,

        /// <summary> 
        /// Jewish: Renewal
        /// </summary>
        [EnumValue("Jewish: Renewal")]
        JRN,

        /// <summary> 
        /// Muslim
        /// </summary>
        [EnumValue("Muslim")]
        MOS,

        /// <summary> 
        /// Muslim: Sunni
        /// </summary>
        [EnumValue("Muslim: Sunni")]
        MSU,

        /// <summary> 
        /// Muslim: Shiite
        /// </summary>
        [EnumValue("Muslim: Shiite")]
        MSH,

        /// <summary> 
        /// Muslim: Other
        /// </summary>
        [EnumValue("Muslim: Other")]
        MOT,

        /// <summary> 
        /// Native American
        /// </summary>
        [EnumValue("Native American")]
        NAM,

        /// <summary> 
        /// New Religionist
        /// </summary>
        [EnumValue("New Religionist")]
        NRL,

        /// <summary> 
        /// Nonreligious
        /// </summary>
        [EnumValue("Nonreligious")]
        NOE,

        /// <summary> 
        /// Other
        /// </summary>
        [EnumValue("Other")]
        OTH,

        /// <summary> 
        /// Shintoist
        /// </summary>
        [EnumValue("Shintoist")]
        SHN,

        /// <summary> 
        /// Sikh
        /// </summary>
        [EnumValue("Sikh")]
        SIK,

        /// <summary> 
        /// Spiritist
        /// </summary>
        [EnumValue("Spiritist")]
        SPI
    }
}