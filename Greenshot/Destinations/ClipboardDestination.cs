﻿/*
 * Greenshot - a free and open source screenshot tool
 * Copyright (C) 2007-2015 Thomas Braun, Jens Klingen, Robin Krom
 * 
 * For more information see: http://getgreenshot.org/
 * The Greenshot project is hosted on Sourceforge: http://sourceforge.net/projects/greenshot/
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 1 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */
using System;
using System.Drawing;
using System.Windows.Forms;

using Greenshot.Configuration;
using GreenshotPlugin.Core;
using Greenshot.Plugin;
using Greenshot.IniFile;
using log4net;
using System.Threading.Tasks;
using System.Threading;

namespace Greenshot.Destinations {
	/// <summary>
	/// Description of ClipboardDestination.
	/// </summary>
	public class ClipboardDestination : AbstractDestination {
		private static ILog LOG = LogManager.GetLogger(typeof(ClipboardDestination));
		private static CoreConfiguration conf = IniConfig.GetIniSection<CoreConfiguration>();
		public const string DESIGNATION = "Clipboard";

		public override string Designation {
			get {
				return DESIGNATION;
			}
		}

		public override string Description {
			get {
				return Language.GetString(LangKey.settings_destination_clipboard);
			}
		}
		public override int Priority {
			get {
				return 2;
			}
		}
		
		public override Keys EditorShortcutKeys {
			get {
				return Keys.Control | Keys.Shift | Keys.C;
			}
		}

		public override Image DisplayIcon {
			get {
				return GreenshotResources.getImage("Clipboard.Image");
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="manuallyInitiated"></param>
		/// <param name="surface"></param>
		/// <param name="captureDetails"></param>
		/// <param name="token"></param>
		/// <returns></returns>
		public override async Task<ExportInformation> ExportCaptureAsync(bool manuallyInitiated, ISurface surface, ICaptureDetails captureDetails, CancellationToken token = default(CancellationToken)) {
			var exportInformation = new ExportInformation(Designation, Description);
			try {
				// There is not much that can work async for the Clipboard
				await Task.Factory.StartNew(() => {
					ClipboardHelper.SetClipboardData(surface);
				}, default(CancellationToken), TaskCreationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
				exportInformation.ExportMade = true;
			} catch (Exception) {
				exportInformation.ErrorMessage = Language.GetString(LangKey.editor_clipboardfailed);
			}

			ProcessExport(exportInformation, surface);
			return exportInformation;
		}
	}
}
