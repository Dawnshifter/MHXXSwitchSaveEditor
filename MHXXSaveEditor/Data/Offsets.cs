namespace MHXXSaveEditor.Data
{
    class Offsets
    {
        //Header Data
        public const int FIRST_CHAR_SLOT_USED = 0x04; //Size 1
        public const int SECOND_CHAR_SLOT_USED = 0x05; //Size 1
        public const int THIRD_CHAR_SLOT_USED = 0x06; //Size 1
        public const int FIRST_CHARACTER_OFFSET = 0x10; //Size 4
        public const int SECOND_CHARACTER_OFFSET = 0x14; //Size 4
        public const int THIRD_CHARACTER_OFFSET = 0x18; //Size 4

        //Character Offsets [CHARACTER BASE +  CHARACTER OFFSET]
        public const int NAME_OFFSET = 0x23B7D; // size 4
        public const int PLAY_TIME_OFFSET = 0x20; //Size 4m this only shows on the save screen
        public const int PLAY_TIME_OFFSET2 = 0x2248B; //Size 4
        public const int FUNDS_OFFSET = 0x24; //Size 4, this only shows on the save screen
        public const int FUNDS_OFFSET2 = 0x280F; // size 4
        public const int HUNTER_RANK_OFFSET = 0x28; //Size 2 
        public const int CHARACTER_VOICE_OFFSET = 0x23B48; //Size 1
        public const int CHARACTER_EYE_COLOR_OFFSET = 0x23B49; //Size 1
        public const int CHARACTER_CLOTHING_OFFSET = 0x23B4A; //Size 1
        public const int CHARACTER_GENDER_OFFSET = 0x23B4B; // Size 1
        public const int CHARACTER_HUNTINGSTYLE_OFFSET = 0x23B4C; // Size 1
        public const int CHARACTER_HAIRSTYLE_OFFSET = 0x23B4D; //Size 1
        public const int CHARACTER_FACE_OFFSET = 0x23B4E; //Size 1
        public const int CHARACTER_FEATURES_OFFSET = 0x23B4F; //Size 1
        public const int CHARACTER_SKIN_COLOR_OFFSET = 0x23B67; //Size 4 
        public const int CHARACTER_HAIR_COLOR_OFFSET = 0X23B6B; //Size 4
        public const int CHARACTER_FEATURES_COLOR_OFFSET = 0x23B6F; //Size 4
        public const int CHARACTER_CLOTHING_COLOR_OFFSET = 0X23B73; //Size 4

        // Only shown on save screen ?
        //public const int NAME_OFFSET = 0x0; //Size 32
        //public const int HUNTER_ART_1_OFFSET = 0x2C; //Size 2
        //public const int HUNTER_ART_2_OFFSET = 0x2E; //Size 2
        //public const int HUNTER_ART_3_OFFSET = 0X30; //Size 2
        //public const int EQUIPPED_WEAPON_OFFSET = 0x010C; //Size 48
        //public const int EQUIPPED_HEAD_OFFSET = 0x013C; //Size 48
        //public const int EQUIPPED_CHEST_OFFSET = 0x016C; //Size 48
        //public const int EQUIPPED_ARMS_OFFSET = 0x019c; //Size 48
        //public const int EQUIPPED_WAIST_OFFSET = 0x01CC; //Size 48
        //public const int EQUIPPED_LEG_OFFSET = 0x01FC; //Size 48
        //public const int EQUIPPED_TALISMAN_OFFSET = 0x022C; //Size 48
        //public const int WEAPON_TYPE_OFFSET = 0x025C; //Size 1
        //public const int CHARACTER_VOICE_OFFSET = 0x241; //Size 1
        //public const int CHARACTER_EYE_COLOR_OFFSET = 0x242; //Size 1
        //public const int CHARACTER_CLOTHING_OFFSET = 0x243; //Size 1
        //public const int CHARACTER_GENDER_OFFSET = 0x244;  //Size 1
        //public const int CHARACTER_HUNTING_STYLE_OFFSET = 0x245; //Size 1
        //public const int CHARACTER_HAIRSTYLE_OFFSET = 0x246; //Size 1
        //public const int CHARACTER_FACE_OFFSET = 0x247; //Size 1
        //public const int CHARACTER_FEATURES_OFFSET = 0x248; //Size 1
        //public const int CHARACTER_SKIN_COLOR_OFFSET = 0x260; //Size 4 
        //public const int CHARACTER_HAIR_COLOR_OFFSET = 0x264; //Size 4
        //public const int CHARACTER_FEATURES_COLOR_OFFSET = 0x268; //Size 4
        //public const int CHARACTER_CLOTHING_COLOR_OFFSET = 0x26C; //Size 4
        //public const int CHEST_ARMOR_PIGMENT_OFFSET = 0x0268; //Size 4
        //public const int ARMS_ARMOR_PIGMENT_OFFSET = 0x026C; //Size 4 
        //public const int WAIST_ARMOR_PIGMENT_OFFSET = 0x0270;  //Size 4
        //public const int LEG_ARMOR_PIGMENT_OFFSET = 0x0274; //Size 4
        //public const int HEAD_ARMOR_PIGMENT_OFFSET = 0x0278; //Size 4

        // Palico
        public const int PALICO_OFFSET = 0x23BB6; //Size 27216 (84 of them each 324 bytes long)

        // Points
        public const int HR_POINTS_OFFSET = 0x280B; //Size 4
        public const int ACADEMY_POINTS_OFFSET = 0x2817; //Size 4
        public const int BHERNA_POINTS_OFFSET = 0x281B; //Size 4
        public const int KOKOTO_POINTS_OFFSET = 0x281F; //Size 4
        public const int POKKE_POINTS_OFFSET = 0x2823; //Size 4
        public const int YUKUMO_POINTS_OFFSET = 0x2827; //Size 4

        // Monster Hunts / Sizes
        public const int MONSTERHUNT_OFFSETS = 0x5EA6; //Size 274, 137 Monsters (supposedly) 2 bytes each
        public const int MONSTERCAPTURE_OFFSETS = 0x5FB8; //Size 274,137 Monsters (supposedly) 2 bytes each
        public const int MONSTERSIZE_OFFSETS = 0x60CA; //Size 548, 4 bytes per monster

        // Items, Equips, Pouch
        public const int ITEM_BOX_OFFSET = 0x0278; //Size 5463 (2300 of them each 19 bits long)
        public const int EQUIPMENT_BOX_OFFSET = 0x62EE; //Size 72000 (2000 of them each 36 bytes long)
        public const int PALICO_EQUIPMENT_OFFSET = 0x17C2E; //Size 36000 (1000 of them 36 bytes long)
        //public const int ITEM_SET_OFFSET = 0x0EDE; //Size 1360 (8 of them each 170 bytes long)
        //public const int POUCH_OFFSET = 0x142E; //Size 72 (32 Items each 18 bits long)

        // Player Guild Card
        public const int GUILCARD_OFFSET = 0xC71BD;
        //public const int GUILDCARD_NAME_OFFSET = 0xC71BD; //Size 12 ?
        //public const int GUILDCARD_WEAPONTYPE_OFFSET = 0XC71D5; //Size 1
        public const int GUILDCARD_VOICE_OFFSET = 0XC71D6; //Size 1
        public const int GUILDCARD_EYE_COLOR_OFFSET = 0XC71D7; //Size 1
        public const int GUILDCARD_CLOTHING_OFFSET = 0XC71D8; //Size 1
        public const int GUILDCARD_GENDER_OFFSET = 0XC71D9; // Size 1
        public const int GUILDCARD_HUNTINGSTYLE_OFFSET = 0xC71DA; // Size 1
        public const int GUILDCARD_HAIRSTYLE_OFFSET = 0xC71DB; //Size 1
        public const int GUILDCARD_FACE_OFFSET = 0xC71DC; //Size 1
        public const int GUILDCARD_FEATURES_OFFSET = 0xC71DD; //Size 1
        public const int GUILDCARD_SKIN_COLOR_OFFSET = 0XC71F5; //Size 4
        public const int GUILDCARD_HAIR_COLOR_OFFSET = 0XC71F9; //Size 4
        public const int GUILDCARD_FEATURES_COLOR_OFFSET = 0XC71FD; //Size 4
        public const int GUILDCARD_CLOTHING_COLOR_OFFSET = 0XC7201; //Size 4
        //public const int GUILDCARD_LOWVIL_QUEST_OFFSET = 0xC7A1B; //Size 2
        //public const int GUILDCARD_HIGHVIL_QUEST_OFFSET = 0xC7A1D; //Size 2
        //public const int GUILDCARD_LOWHUB_QUEST_OFFSET = 0xC7A1F; //Size 2
        //public const int GUILDCARD_HIGHHUB_QUEST_OFFSET = 0xC7A21; //Size 2
        //public const int GUILDCARD_GRANK_QUEST_OFFSET = 0xC7A23; //Size 2
        //public const int GUILDCARD_SP_QUEST_OFFSET = 0xC7A25; //Size 2
        //public const int GUILDCARD_ARENA_QUEST_OFFSET = 0xC7A27; //Size 2
        public const int GUILDCARD_ID_OFFSET = 0XC7A6D; //Size 8
        //public const int GUILDCARD_VILLAGE_WEAPON_OFFSET = 0XC7A77; //Size 30
        //public const int GUILDCARD_HUB_WEAPON_OFFSET = 0XC7A95; //Size 30
        //public const int GUILDCARD_ARENA_WEAPON_OFFSET = 0XC7AB3; //Size 30
        public const int GUILDCARD_ARENA_LOG_OFFSET = 0xC83E1; //Size 324

        // Shoutouts
        public const int MANUAL_SHOUTOUT_OFFSETS = 0x11D629; //Size 60
        public const int AUTOMATIC_SHOUTOUT_OFFSETS = 0x11E169; //Size 60

        //public const int SHOP_OFFSETS = 0x1D76;
        //public const int CRAFTABLE_WEAPONS_OFFSET = 0x20BE;
        //public const int CRAFTABLE_ARMOR_SHOP_OFFSET = 0x2316;
        //public const int CRAFTABLE_PALICO_GEAR_OFFSET = 0x02ABE;

        //public const int FOOD_FLAG_OFFSETS = 0x1A32; //Size 4
        //public const int AWARD_FLAG_OFFSETS = 0x1B8A; //Size 13

        //public const int UNLOCKED_BOXES_OFFSET = 0x1A22; //Size 8
    }
}
