#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.PresentationStates
{
	public class DicomSoftcopyPresentationStateGraphic : CompositeGraphic, IEnumerable<DicomSoftcopyPresentationStateGraphic.LayerGraphic>
	{
		private readonly Dictionary<string, LayerGraphic> _layers;

		public DicomSoftcopyPresentationStateGraphic()
		{
			_layers = new Dictionary<string, LayerGraphic>();
		}

		public LayerGraphic this[string layerId]
		{
			get
			{
				if (!_layers.ContainsKey(layerId))
					this.AddLayer(layerId);
				return _layers[layerId.ToUpperInvariant()];
			}
		}

		public bool HasLayer(string layerId)
		{
			return _layers.ContainsKey(layerId.ToUpperInvariant());
		}

		public int Count
		{
			get { return _layers.Count; }
		}

		public LayerGraphic AddLayer(string layerId)
		{
			layerId = layerId.ToUpperInvariant();
			if (_layers.ContainsKey(layerId))
				return _layers[layerId];

			LayerGraphic layer = new LayerGraphic(layerId);
			_layers.Add(layerId, layer);
			base.Graphics.Add(layer);
			return layer;
		}

		public bool RemoveLayer(string layerId)
		{
			layerId = layerId.ToUpperInvariant();
			if (!_layers.ContainsKey(layerId))
				return false;

			base.Graphics.Remove(_layers[layerId]);
			_layers.Remove(layerId);
			return true;
		}

		public IEnumerator<LayerGraphic> GetEnumerator()
		{
			return _layers.Values.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public static DicomSoftcopyPresentationStateGraphic GetPresentationStateGraphic(IOverlayGraphicsProvider overlayGraphicsProvider, bool createIfNecessary)
		{
			GraphicCollection overlayGraphics = overlayGraphicsProvider.OverlayGraphics;
			DicomSoftcopyPresentationStateGraphic xspsGraphic = CollectionUtils.SelectFirst(overlayGraphics,
			                                                                          delegate(IGraphic graphic) { return graphic is DicomSoftcopyPresentationStateGraphic; }
			                                              	) as DicomSoftcopyPresentationStateGraphic;

			if (xspsGraphic == null && createIfNecessary)
				overlayGraphics.Add(xspsGraphic = new DicomSoftcopyPresentationStateGraphic());

			return xspsGraphic;
		}

		public class LayerGraphic : CompositeGraphic
		{
			private readonly string _id;

			private int[] _displayCIELabColor;
			private int? _displayGrayscaleColor;
			private string _description;

			internal LayerGraphic(string id) {
				_id = id.ToUpperInvariant();
			}

			public string Id
			{
				get { return _id; }
			}

			public string Description
			{
				get { return _description; }
				set { _description = value; }
			}

			public int? DisplayGrayscaleColor
			{
				get { return _displayGrayscaleColor; }
				set { _displayGrayscaleColor = value; }
			}

			public int[] DisplayCIELabColor
			{
				get { return _displayCIELabColor; }
				set { _displayCIELabColor = value; }
			}

			public new DicomSoftcopyPresentationStateGraphic ParentGraphic
			{
				get { return (DicomSoftcopyPresentationStateGraphic) base.ParentGraphic; }
			}
		}
	}
}