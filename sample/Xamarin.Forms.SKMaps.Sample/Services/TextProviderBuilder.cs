﻿// **********************************************************************
// 
//   TextProviderBuilder.cs
//   
//   This file is subject to the terms and conditions defined in
//   file 'LICENSE.txt', which is part of this source code package.
//   
//   Copyright (c) 2018, Le rond-point
// 
// ***********************************************************************

using Xamarin.Forms.SKMaps.Sample.Services.Interface;
using MvvmCross;
using MvvmCross.Base;
using MvvmCross.Plugin.JsonLocalization;
using MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Xamarin.Forms.SKMaps.Sample.Services
{
    public class TextProviderBuilder : MvxTextProviderBuilder
    {
        public event EventHandler LanguageResourcesLoaded;

        internal string _DefaultTypeKey { get; }
        private Dictionary<string, string> _LocalizedTextKeys { get; set; }
        private Dictionary<string, string> _ExtraTextKeys { get; set; } = new Dictionary<string, string>();

        public TextProviderBuilder(string textFolder, string defaultTypeKey)
            : base(Mvx.IoCProvider.Resolve<IResourceLocator>().ResourcesNamespace, textFolder, new MvxEmbeddedJsonDictionaryTextProvider(false))
        {
            _DefaultTypeKey = defaultTypeKey;

            LoadTextResources();
        }

        protected override IDictionary<string, string> ResourceFiles
        {
            get
            {
                Dictionary<string, string> dictionary;

                if (_LocalizedTextKeys == null)
                {
                    BuildViewModelsTextKeys();
                }

                dictionary = new Dictionary<string, string>(_LocalizedTextKeys);
                if (!string.IsNullOrEmpty(_DefaultTypeKey))
                {
                    dictionary.Add(_DefaultTypeKey, _DefaultTypeKey);
                }

                if (_ExtraTextKeys.Any())
                {
                    foreach (KeyValuePair<string, string> kvp in _ExtraTextKeys)
                    {
                        dictionary.Add(kvp.Key, kvp.Value);
                    }
                }

                if (LanguageResourcesLoaded != null)
                {
                    Mvx.IoCProvider.Resolve<IMvxMainThreadAsyncDispatcher>().ExecuteOnMainThreadAsync(() => LanguageResourcesLoaded?.Invoke(this, EventArgs.Empty));
                }

                return dictionary;
            }
        }

        private void BuildViewModelsTextKeys()
        {
            IEnumerable<TypeInfo> localizedViewModelTypes = Mvx.IoCProvider.Resolve<IResourceLocator>()
                                                                           .ResourcesAssembly
                                                                           .DefinedTypes
                                                                           .Where(t => t.IsSubclassOf(typeof(MvxViewModel)));
            string namespacePrefix = Mvx.IoCProvider.Resolve<IResourceLocator>()
                                                    .ResourcesNamespace + ".";

            _LocalizedTextKeys = new Dictionary<string, string>();
            foreach (TypeInfo type in localizedViewModelTypes)
            {
                string typeKey = type.FullName.Replace(namespacePrefix, "");

                _LocalizedTextKeys.Add(typeKey, typeKey);
            }
        }

        public void RegisterExtraTextKeys(IDictionary<string, string> extraKeys)
        {
            _ExtraTextKeys = new Dictionary<string, string>(extraKeys);

            LoadTextResources();
        }

        private void LoadTextResources()
        {
            LoadResources(string.Empty); // Load default text
            LoadResources(CultureInfo.CurrentUICulture.TwoLetterISOLanguageName); // Override with language specific text
        }
    }
}