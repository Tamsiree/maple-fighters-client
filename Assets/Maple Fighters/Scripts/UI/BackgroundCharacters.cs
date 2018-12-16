﻿using Scripts.UI.Core;

namespace Scripts.UI
{
    public class BackgroundCharacters : UniqueUserInterfaceBase
    {
        public override void Hide()
        {
            UserInterfaceContainer.GetInstance()?.Remove(this);
        }
    }
}