﻿/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/
using System.Collections.Generic;

namespace WooLocalization
{
    public interface IActorContext<Value>
    {
        Value GetLocalization(string localizationType, string key);
        List<string> GetLocalizationKeys();
        List<string> GetLocalizationTypes();
    }
}
