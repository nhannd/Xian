using System;
using System.Collections;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Enterprise;

namespace ClearCanvas.Healthcare {

    /// <summary>
    /// SpokenLanguage enumeration
    /// </summary>
	public enum SpokenLanguage
	{
        /// <summary> 
        /// English
        /// </summary>
        [EnumValue("English")]
        en,

        /// <summary> 
        /// Afar
        /// </summary>
        [EnumValue("Afar")]
        aa,

        /// <summary> 
        /// Abkhazian
        /// </summary>
        [EnumValue("Abkhazian")]
        ab,

        /// <summary> 
        /// Afrikaans
        /// </summary>
        [EnumValue("Afrikaans")]
        af,

        /// <summary> 
        /// Amharic
        /// </summary>
        [EnumValue("Amharic")]
        am,

        /// <summary> 
        /// Arabic
        /// </summary>
        [EnumValue("Arabic")]
        ar,

        /// <summary> 
        /// Assamese
        /// </summary>
        [EnumValue("Assamese")]
        @as,

        /// <summary> 
        /// Aymara
        /// </summary>
        [EnumValue("Aymara")]
        ay,

        /// <summary> 
        /// Azerbaijani
        /// </summary>
        [EnumValue("Azerbaijani")]
        az,

        /// <summary> 
        /// Bashkir
        /// </summary>
        [EnumValue("Bashkir")]
        ba,

        /// <summary> 
        /// Byelorussian
        /// </summary>
        [EnumValue("Byelorussian")]
        be,

        /// <summary> 
        /// Bulgarian
        /// </summary>
        [EnumValue("Bulgarian")]
        bg,

        /// <summary> 
        /// Bihari
        /// </summary>
        [EnumValue("Bihari")]
        bh,

        /// <summary> 
        /// Bislama
        /// </summary>
        [EnumValue("Bislama")]
        bi,

        /// <summary> 
        /// Bengali; Bangla
        /// </summary>
        [EnumValue("Bengali; Bangla")]
        bn,

        /// <summary> 
        /// Tibetan
        /// </summary>
        [EnumValue("Tibetan")]
        bo,

        /// <summary> 
        /// Breton
        /// </summary>
        [EnumValue("Breton")]
        br,

        /// <summary> 
        /// Catalan
        /// </summary>
        [EnumValue("Catalan")]
        ca,

        /// <summary> 
        /// Corsican
        /// </summary>
        [EnumValue("Corsican")]
        co,

        /// <summary> 
        /// Czech
        /// </summary>
        [EnumValue("Czech")]
        cs,

        /// <summary> 
        /// Welsh
        /// </summary>
        [EnumValue("Welsh")]
        cy,

        /// <summary> 
        /// Danish
        /// </summary>
        [EnumValue("Danish")]
        da,

        /// <summary> 
        /// German
        /// </summary>
        [EnumValue("German")]
        de,

        /// <summary> 
        /// Bhutani
        /// </summary>
        [EnumValue("Bhutani")]
        dz,

        /// <summary> 
        /// Greek
        /// </summary>
        [EnumValue("Greek")]
        el,

        /// <summary> 
        /// Esperanto
        /// </summary>
        [EnumValue("Esperanto")]
        eo,

        /// <summary> 
        /// Spanish
        /// </summary>
        [EnumValue("Spanish")]
        es,

        /// <summary> 
        /// Estonian
        /// </summary>
        [EnumValue("Estonian")]
        et,

        /// <summary> 
        /// Basque
        /// </summary>
        [EnumValue("Basque")]
        eu,

        /// <summary> 
        /// Persian
        /// </summary>
        [EnumValue("Persian")]
        fa,

        /// <summary> 
        /// Finnish
        /// </summary>
        [EnumValue("Finnish")]
        fi,

        /// <summary> 
        /// Fiji
        /// </summary>
        [EnumValue("Fiji")]
        fj,

        /// <summary> 
        /// Faroese
        /// </summary>
        [EnumValue("Faroese")]
        fo,

        /// <summary> 
        /// French
        /// </summary>
        [EnumValue("French")]
        fr,

        /// <summary> 
        /// Frisian
        /// </summary>
        [EnumValue("Frisian")]
        fy,

        /// <summary> 
        /// Irish
        /// </summary>
        [EnumValue("Irish")]
        ga,

        /// <summary> 
        /// Scots Gaelic
        /// </summary>
        [EnumValue("Scots Gaelic")]
        gd,

        /// <summary> 
        /// Galician
        /// </summary>
        [EnumValue("Galician")]
        gl,

        /// <summary> 
        /// Guarani
        /// </summary>
        [EnumValue("Guarani")]
        gn,

        /// <summary> 
        /// Gujarati
        /// </summary>
        [EnumValue("Gujarati")]
        gu,

        /// <summary> 
        /// Hausa
        /// </summary>
        [EnumValue("Hausa")]
        ha,

        /// <summary> 
        /// Hebrew (formerly iw)
        /// </summary>
        [EnumValue("Hebrew (formerly iw)")]
        he,

        /// <summary> 
        /// Hindi
        /// </summary>
        [EnumValue("Hindi")]
        hi,

        /// <summary> 
        /// Croatian
        /// </summary>
        [EnumValue("Croatian")]
        hr,

        /// <summary> 
        /// Hungarian
        /// </summary>
        [EnumValue("Hungarian")]
        hu,

        /// <summary> 
        /// Armenian
        /// </summary>
        [EnumValue("Armenian")]
        hy,

        /// <summary> 
        /// Interlingua
        /// </summary>
        [EnumValue("Interlingua")]
        ia,

        /// <summary> 
        /// Indonesian (formerly in)
        /// </summary>
        [EnumValue("Indonesian (formerly in)")]
        id,

        /// <summary> 
        /// Interlingue
        /// </summary>
        [EnumValue("Interlingue")]
        ie,

        /// <summary> 
        /// Inupiak
        /// </summary>
        [EnumValue("Inupiak")]
        ik,

        /// <summary> 
        /// Icelandic
        /// </summary>
        [EnumValue("Icelandic")]
        @is,

        /// <summary> 
        /// Italian
        /// </summary>
        [EnumValue("Italian")]
        it,

        /// <summary> 
        /// Inuktitut
        /// </summary>
        [EnumValue("Inuktitut")]
        iu,

        /// <summary> 
        /// Japanese
        /// </summary>
        [EnumValue("Japanese")]
        ja,

        /// <summary> 
        /// Javanese
        /// </summary>
        [EnumValue("Javanese")]
        jw,

        /// <summary> 
        /// Georgian
        /// </summary>
        [EnumValue("Georgian")]
        ka,

        /// <summary> 
        /// Kazakh
        /// </summary>
        [EnumValue("Kazakh")]
        kk,

        /// <summary> 
        /// Greenlandic
        /// </summary>
        [EnumValue("Greenlandic")]
        kl,

        /// <summary> 
        /// Cambodian
        /// </summary>
        [EnumValue("Cambodian")]
        km,

        /// <summary> 
        /// Kannada
        /// </summary>
        [EnumValue("Kannada")]
        kn,

        /// <summary> 
        /// Korean
        /// </summary>
        [EnumValue("Korean")]
        ko,

        /// <summary> 
        /// Kashmiri
        /// </summary>
        [EnumValue("Kashmiri")]
        ks,

        /// <summary> 
        /// Kurdish
        /// </summary>
        [EnumValue("Kurdish")]
        ku,

        /// <summary> 
        /// Kirghiz
        /// </summary>
        [EnumValue("Kirghiz")]
        ky,

        /// <summary> 
        /// Latin
        /// </summary>
        [EnumValue("Latin")]
        la,

        /// <summary> 
        /// Lingala
        /// </summary>
        [EnumValue("Lingala")]
        ln,

        /// <summary> 
        /// Laothian
        /// </summary>
        [EnumValue("Laothian")]
        lo,

        /// <summary> 
        /// Lithuanian
        /// </summary>
        [EnumValue("Lithuanian")]
        lt,

        /// <summary> 
        /// Latvian Lettish
        /// </summary>
        [EnumValue("Latvian Lettish")]
        lv,

        /// <summary> 
        /// Malagasy
        /// </summary>
        [EnumValue("Malagasy")]
        mg,

        /// <summary> 
        /// Maori
        /// </summary>
        [EnumValue("Maori")]
        mi,

        /// <summary> 
        /// Macedonian
        /// </summary>
        [EnumValue("Macedonian")]
        mk,

        /// <summary> 
        /// Malayalam
        /// </summary>
        [EnumValue("Malayalam")]
        ml,

        /// <summary> 
        /// Mongolian
        /// </summary>
        [EnumValue("Mongolian")]
        mn,

        /// <summary> 
        /// Moldavian
        /// </summary>
        [EnumValue("Moldavian")]
        mo,

        /// <summary> 
        /// Marathi
        /// </summary>
        [EnumValue("Marathi")]
        mr,

        /// <summary> 
        /// Malay
        /// </summary>
        [EnumValue("Malay")]
        ms,

        /// <summary> 
        /// Maltese
        /// </summary>
        [EnumValue("Maltese")]
        mt,

        /// <summary> 
        /// Burmese
        /// </summary>
        [EnumValue("Burmese")]
        my,

        /// <summary> 
        /// Nauru
        /// </summary>
        [EnumValue("Nauru")]
        na,

        /// <summary> 
        /// Nepali
        /// </summary>
        [EnumValue("Nepali")]
        ne,

        /// <summary> 
        /// Dutch
        /// </summary>
        [EnumValue("Dutch")]
        nl,

        /// <summary> 
        /// Norwegian
        /// </summary>
        [EnumValue("Norwegian")]
        no,

        /// <summary> 
        /// Occitan
        /// </summary>
        [EnumValue("Occitan")]
        oc,

        /// <summary> 
        /// (Afan) Oromo
        /// </summary>
        [EnumValue("(Afan) Oromo")]
        om,

        /// <summary> 
        /// Oriya
        /// </summary>
        [EnumValue("Oriya")]
        or,

        /// <summary> 
        /// Punjabi
        /// </summary>
        [EnumValue("Punjabi")]
        pa,

        /// <summary> 
        /// Polish
        /// </summary>
        [EnumValue("Polish")]
        pl,

        /// <summary> 
        /// Pashto Pushto
        /// </summary>
        [EnumValue("Pashto Pushto")]
        ps,

        /// <summary> 
        /// Portuguese
        /// </summary>
        [EnumValue("Portuguese")]
        pt,

        /// <summary> 
        /// Quechua
        /// </summary>
        [EnumValue("Quechua")]
        qu,

        /// <summary> 
        /// Rhaeto-Romance
        /// </summary>
        [EnumValue("Rhaeto-Romance")]
        rm,

        /// <summary> 
        /// Kirundi
        /// </summary>
        [EnumValue("Kirundi")]
        rn,

        /// <summary> 
        /// Romanian
        /// </summary>
        [EnumValue("Romanian")]
        ro,

        /// <summary> 
        /// Russian
        /// </summary>
        [EnumValue("Russian")]
        ru,

        /// <summary> 
        /// Kinyarwanda
        /// </summary>
        [EnumValue("Kinyarwanda")]
        rw,

        /// <summary> 
        /// Sanskrit
        /// </summary>
        [EnumValue("Sanskrit")]
        sa,

        /// <summary> 
        /// Sindhi
        /// </summary>
        [EnumValue("Sindhi")]
        sd,

        /// <summary> 
        /// Sangho
        /// </summary>
        [EnumValue("Sangho")]
        sg,

        /// <summary> 
        /// Serbo-Croatian
        /// </summary>
        [EnumValue("Serbo-Croatian")]
        sh,

        /// <summary> 
        /// Sinhalese
        /// </summary>
        [EnumValue("Sinhalese")]
        si,

        /// <summary> 
        /// Slovak
        /// </summary>
        [EnumValue("Slovak")]
        sk,

        /// <summary> 
        /// Slovenian
        /// </summary>
        [EnumValue("Slovenian")]
        sl,

        /// <summary> 
        /// Samoan
        /// </summary>
        [EnumValue("Samoan")]
        sm,

        /// <summary> 
        /// Shona
        /// </summary>
        [EnumValue("Shona")]
        sn,

        /// <summary> 
        /// Somali
        /// </summary>
        [EnumValue("Somali")]
        so,

        /// <summary> 
        /// Albanian
        /// </summary>
        [EnumValue("Albanian")]
        sq,

        /// <summary> 
        /// Serbian
        /// </summary>
        [EnumValue("Serbian")]
        sr,

        /// <summary> 
        /// Siswati
        /// </summary>
        [EnumValue("Siswati")]
        ss,

        /// <summary> 
        /// Sesotho
        /// </summary>
        [EnumValue("Sesotho")]
        st,

        /// <summary> 
        /// Sundanese
        /// </summary>
        [EnumValue("Sundanese")]
        su,

        /// <summary> 
        /// Swedish
        /// </summary>
        [EnumValue("Swedish")]
        sv,

        /// <summary> 
        /// Swahili
        /// </summary>
        [EnumValue("Swahili")]
        sw,

        /// <summary> 
        /// Tamil
        /// </summary>
        [EnumValue("Tamil")]
        ta,

        /// <summary> 
        /// Telugu
        /// </summary>
        [EnumValue("Telugu")]
        te,

        /// <summary> 
        /// Tajik
        /// </summary>
        [EnumValue("Tajik")]
        tg,

        /// <summary> 
        /// Thai
        /// </summary>
        [EnumValue("Thai")]
        th,

        /// <summary> 
        /// Tigrinya
        /// </summary>
        [EnumValue("Tigrinya")]
        ti,

        /// <summary> 
        /// Turkmen
        /// </summary>
        [EnumValue("Turkmen")]
        tk,

        /// <summary> 
        /// Tagalog
        /// </summary>
        [EnumValue("Tagalog")]
        tl,

        /// <summary> 
        /// Setswana
        /// </summary>
        [EnumValue("Setswana")]
        tn,

        /// <summary> 
        /// Tonga
        /// </summary>
        [EnumValue("Tonga")]
        to,

        /// <summary> 
        /// Turkish
        /// </summary>
        [EnumValue("Turkish")]
        tr,

        /// <summary> 
        /// Tsonga
        /// </summary>
        [EnumValue("Tsonga")]
        ts,

        /// <summary> 
        /// Tatar
        /// </summary>
        [EnumValue("Tatar")]
        tt,

        /// <summary> 
        /// Twi
        /// </summary>
        [EnumValue("Twi")]
        tw,

        /// <summary> 
        /// Uighur
        /// </summary>
        [EnumValue("Uighur")]
        ug,

        /// <summary> 
        /// Ukrainian
        /// </summary>
        [EnumValue("Ukrainian")]
        uk,

        /// <summary> 
        /// Urdu
        /// </summary>
        [EnumValue("Urdu")]
        ur,

        /// <summary> 
        /// Uzbek
        /// </summary>
        [EnumValue("Uzbek")]
        uz,

        /// <summary> 
        /// Vietnamese
        /// </summary>
        [EnumValue("Vietnamese")]
        vi,

        /// <summary> 
        /// Volapuk
        /// </summary>
        [EnumValue("Volapuk")]
        vo,

        /// <summary> 
        /// Wolof
        /// </summary>
        [EnumValue("Wolof")]
        wo,

        /// <summary> 
        /// Xhosa
        /// </summary>
        [EnumValue("Xhosa")]
        xh,

        /// <summary> 
        /// Yiddish (formerly ji)
        /// </summary>
        [EnumValue("Yiddish (formerly ji)")]
        yi,

        /// <summary> 
        /// Yoruba
        /// </summary>
        [EnumValue("Yoruba")]
        yo,

        /// <summary> 
        /// Zhuang
        /// </summary>
        [EnumValue("Zhuang")]
        za,

        /// <summary> 
        /// Chinese
        /// </summary>
        [EnumValue("Chinese")]
        zh,

        /// <summary> 
        /// Zulu
        /// </summary>
        [EnumValue("Zulu")]
        zu
	}
}