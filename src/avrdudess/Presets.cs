﻿/*
 * Project: AVRDUDESS - A GUI for AVRDUDE
 * Author: Zak Kemble, contact@zakkemble.co.uk
 * Copyright: (C) 2013 by Zak Kemble
 * License: GNU GPL v3 (see License.txt)
 * Web: http://blog.zakkemble.co.uk/avrdudess-a-gui-for-avrdude/
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace avrdudess
{
    public class Presets
    {
        private const string FILE_PRESETS = "presets.xml";

        private Form1 mainForm;
        private List<PresetData> presetList;
        private XmlFile xmlFile;

        public List<PresetData> presets
        {
            get { return presetList; }
        }

        public Presets(Form1 mainForm)
        {
            this.mainForm = mainForm;
            presetList = new List<PresetData>();
            xmlFile = new XmlFile(FILE_PRESETS, "presets");
        }

        public void setDataSource(ComboBox cb, EventHandler handler)
        {
            cb.SelectedIndexChanged -= handler;
            setDataSource(cb);
            cb.SelectedIndexChanged += handler;
        }

        public void setDataSource(ComboBox cb)
        {
            cb.DataSource = null;
            cb.ValueMember = null;
            cb.BindingContext = new BindingContext();
            cb.DataSource = presetList;
            cb.DisplayMember = "name";
            cb.SelectedIndex = -1;
        }

        // New preset
        public void add(string name)
        {
            presetList.Add(new PresetData(mainForm, name));
            bumpDefault();
        }

        // Delete preset
        public void remove(PresetData preset)
        {
            presetList.Remove(preset);
            bumpDefault();
        }

        // Make sure default is at the top
        private void bumpDefault()
        {
            int idx = presetList.FindIndex(s => s.name == "Default");
            if (idx > -1)
            {
                PresetData p = presetList[idx];
                presetList.RemoveAt(idx);
                presetList.Insert(0, p);
            }
        }

        // Save presets
        public void save()
        {
            xmlFile.save(presets);
        }

        // Load presets
        public void load()
        {
            // If file doesn't exist then make it
            if (!File.Exists(xmlFile.fileLoc))
            {
                add("Default");
                save();
            }

            // Load presets from XML
            presetList = xmlFile.load<List<PresetData>>();
            if (presetList == null)
                presetList = new List<PresetData>();
        }
    }

    [Serializable]
    [XmlType(TypeName = "Preset")] // For backwards compatability with old (<v1.3) presets.xml
    public class PresetData
    {
        public string name { get; set; }

        public string programmer;
        public string mcu;
        public string port;
        public string baud;
        public string bitclock;
        public string flashFile;
        public string flashFormat;
        public string flashOp;
        public string EEPROMFile;
        public string EEPROMFormat;
        public string EEPROMOp;
        public bool force;
        public bool disableVerify;
        public bool disableFlashErase;
        public bool eraseFlashAndEEPROM;
        public bool doNotWrite;
        public string lfuse;
        public string hfuse;
        public string efuse;
        public bool setFuses;
        public string lockBits;
        public bool setLock;
        public string additional;
        public byte verbosity;

        public PresetData()
        {
        }

        public PresetData(Form1 mainForm, string name)
        {
            this.name = name;

            programmer = mainForm.prog;
            mcu = mainForm.mcu;
            port = mainForm.port;
            baud = mainForm.baudRate;
            bitclock = mainForm.bitClock;
            flashFile = mainForm.flashFile;
            flashFormat = mainForm.flashFileFormat;
            flashOp = mainForm.flashFileOperation;
            EEPROMFile = mainForm.EEPROMFile;
            EEPROMFormat = mainForm.EEPROMFileFormat;
            EEPROMOp = mainForm.EEPROMFileOperation;
            force = mainForm.force;
            disableVerify = mainForm.disableVerify;
            disableFlashErase = mainForm.disableFlashErase;
            eraseFlashAndEEPROM = mainForm.eraseFlashAndEEPROM;
            doNotWrite = mainForm.doNotWrite;
            lfuse = mainForm.lowFuse;
            hfuse = mainForm.highFuse;
            efuse = mainForm.exFuse;
            setFuses = mainForm.setFuses;
            lockBits = mainForm.lockSetting;
            setLock = mainForm.setLock;
            additional = mainForm.additionalSettings;
            verbosity = mainForm.verbosity;
        }

        public void load(Form1 mainForm)
        {
            mainForm.prog = programmer;
            mainForm.mcu = mcu;
            mainForm.port = port;
            mainForm.baudRate = baud;
            mainForm.bitClock = bitclock;
            mainForm.flashFile = flashFile;
            mainForm.flashFileFormat = flashFormat;
            mainForm.flashFileOperation = flashOp;
            mainForm.EEPROMFile = EEPROMFile;
            mainForm.EEPROMFileFormat = EEPROMFormat;
            mainForm.EEPROMFileOperation = EEPROMOp;
            mainForm.force = force;
            mainForm.disableVerify = disableVerify;
            mainForm.disableFlashErase = disableFlashErase;
            mainForm.eraseFlashAndEEPROM = eraseFlashAndEEPROM;
            mainForm.doNotWrite = doNotWrite;
            mainForm.lowFuse = lfuse;
            mainForm.highFuse = hfuse;
            mainForm.exFuse = efuse;
            mainForm.setFuses = setFuses;
            mainForm.lockSetting = lockBits;
            mainForm.setLock = setLock;
            mainForm.additionalSettings = additional;
            mainForm.verbosity = verbosity;
        }
    }
}
