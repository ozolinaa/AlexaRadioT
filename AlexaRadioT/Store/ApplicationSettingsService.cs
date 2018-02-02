using AlexaRadioT.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlexaRadioT.Store
{
    public class ApplicationSettingsService
    {
        private static SkillSettings _skillSettings;

        public ApplicationSettingsService(SkillSettings skillSettings)
        {
            _skillSettings = skillSettings;
        }

        public static void SetSkillSettings(SkillSettings skillSettings) {
            _skillSettings = skillSettings;
        }

        public static SkillSettings Skill {
            get {
                return _skillSettings;
            }
        }
    }
}
