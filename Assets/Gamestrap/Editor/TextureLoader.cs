using UnityEngine;
using System.Collections;
using UnityEditor;
using Gamestrap;
using System.IO;

namespace Gamestrap
{
    public class TextureLoader
    {
        private static string path;
        private static Texture2D editIcon, exitIcon, addIcon, tableIcon, searchIcon;
        private static Texture2D paletteNormal, paletteSelected, palettePressed;

        private static void LoadPath()
        {
            string[] assets = AssetDatabase.FindAssets("t:Texture gamestrap_");

            if (assets.Length == 0)
            {
                Debug.LogError("GamestrapUI name not found, make sure you have the Gamestrap scripts in your project.");
                return;
            }

            path = AssetDatabase.GUIDToAssetPath(assets[0]);
            DirectoryInfo dir = Directory.GetParent(path);
            path = "Assets" + dir.FullName.Substring(Application.dataPath.Length) + "\\";
        }

        #region Static Properties
        private static Texture2D LoadTexture(string assetName)
        {

            if (path == null || path.Length == 0)
                LoadPath();
            return (Texture2D) AssetDatabase.LoadAssetAtPath(path + assetName,typeof(Texture2D)); ;
        }

        public static Texture2D AddIcon
        {
            get
            {
                if (addIcon == null)
                    addIcon = LoadTexture("gamestrap_icon_add.png");
                return addIcon;
            }
        }

        public static Texture2D EditIcon
        {
            get
            {
                if (editIcon == null)
                    editIcon = LoadTexture("gamestrap_icon_edit.png");
                return editIcon;
            }
        }

        public static Texture2D ExitIcon
        {
            get
            {
                if (exitIcon == null)
                    exitIcon = LoadTexture("gamestrap_icon_exit.png");
                return exitIcon;
            }
        }

        public static Texture2D SearchIcon
        {
            get
            {
                if (searchIcon == null)
                    searchIcon = LoadTexture("gamestrap_icon_search.png");
                return searchIcon;
            }
        }

        public static Texture2D TableIcon
        {
            get
            {
                if (tableIcon == null)
                    tableIcon = LoadTexture("gamestrap_icon_table.png");
                return tableIcon;
            }
        }

        public static Texture2D PaletteNormal
        {
            get
            {
                if (paletteNormal == null)
                    paletteNormal = LoadTexture("gamestrap_palette_normal.psd");
                return paletteNormal;
            }
        }

        public static Texture2D PaletteSelected
        {
            get
            {
                if (paletteSelected == null)
                    paletteSelected = LoadTexture("gamestrap_palette_selected.psd");
                return paletteSelected;
            }
        }

        public static Texture2D PalettePressed
        {
            get
            {
                if (palettePressed == null)
                    palettePressed = LoadTexture("gamestrap_palette_pressed.psd");
                return palettePressed;
            }
        }
        #endregion
    }
}