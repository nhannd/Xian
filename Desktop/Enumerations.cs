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

using System;

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// Enumeration of (potentially) available mouse buttons.
	/// </summary>
	[Flags]
	public enum XMouseButtons
	{
		/// <summary>
		/// Default value
		/// </summary>
		None		= 0x00000000,
		/// <summary>
		/// The left mouse button.
		/// </summary>
		Left		= 0x00100000,
		/// <summary>
		/// The right mouse button.
		/// </summary>
		Right		= 0x00200000,
		/// <summary>
		/// The middle mouse button.
		/// </summary>
		Middle		= 0x00400000,
		/// <summary>
		/// The 'x1' button.
		/// </summary>
		XButton1	= 0x00800000,
		/// <summary>
		/// The 'x2' button.
		/// </summary>
		XButton2	= 0x01000000
	}

	/// <summary>
	/// Enumeration for keyboard modifiers.
	/// </summary>
	[Flags]
	public enum ModifierFlags
	{
		/// <summary>
		/// Default value.
		/// </summary>
		None = 0,
		/// <summary>
		/// Any one of the 'control' keys.
		/// </summary>
		Control = 1,
		/// <summary>
		/// Any one of the 'alt' keys.
		/// </summary>
		Alt = 2,
		/// <summary>
		/// Any one of the 'shift' keys.
		/// </summary>
		Shift = 4
	}

	/// <summary>
	/// Enumeration of all the (potentially) available keys on a keyboard.
	/// </summary>
	[Flags]
	public enum XKeys
	{
#pragma warning disable 1591
		A = 65,
		Add = 107,
		Alt = 262144,
		Apps = 93,
		Attn = 246,
		B = 66,
		Back = 8,
		BrowserBack = 166,
		BrowserFavorites = 171,
		BrowserForward = 167,
		BrowserHome = 172,
		BrowserRefresh = 168,
		BrowserSearch = 170,
		BrowserStop = 169,
		C = 67,
		Cancel = 3,
		Capital = 20,
		CapsLock = 20,
		Clear = 12,
		Control = 131072,
		ControlKey = 17,
		Crsel = 247,
		D = 68,
		D0 = 48,
		D1 = 49,
		D2 = 50,
		D3 = 51,
		D4 = 52,
		D5 = 53,
		D6 = 54,
		D7 = 55,
		D8 = 56,
		D9 = 57,
		Decimal = 110,
		Delete = 46,
		Divide = 111,
		Down = 40,
		E = 69,
		End = 35,
		Enter = 13,
		EraseEof = 249,
		Escape = 27,
		Execute = 43,
		Exsel = 248,
		F = 70,
		F1 = 112,
		F2 = 113,
		F3 = 114,
		F4 = 115,
		F5 = 116,
		F6 = 117,
		F7 = 118,
		F8 = 119,
		F9 = 120,
		F10 = 121,
		F11 = 122,
		F12 = 123,
		F13 = 124,
		F14 = 125,
		F15 = 126,
		F16 = 127,
		F17 = 128,
		F18 = 129,
		F19 = 130,
		F20 = 131,
		F21 = 132,
		F22 = 133,
		F23 = 134,
		F24 = 135,
		FinalMode = 24,
		G = 71,
		H = 72,
		HanguelMode = 21,
		HangulMode = 21,
		HanjaMode = 25,
		Help = 47,
		Home = 36,
		I = 73,
		IMEAceept = 30,
		IMEConvert = 28,
		IMEModeChange = 31,
		IMENonconvert = 29,
		Insert = 45,
		J = 74,
		JunjaMode = 23,
		K = 75,
		KanaMode = 21,
		KanjiMode = 25,
		KeyCode = 65535,
		L = 76,
		LaunchApplication1 = 182,
		LaunchApplication2 = 183,
		LaunchMail = 180,
		LButton = 1,
		LControlKey = 162,
		Left = 37,
		LineFeed = 10,
		LShiftKey = 160,
		LMenu = 164,
		LWin = 91,
		M = 77,
		MButton = 4,
		MediaNextTrack = 176,
		MediaPlayPause = 179,
		MediaPreviousTrack = 177,
		MediaStop = 178,
		Menu = 18,
		Modifiers = -65536,
		Multiply = 106,
		N = 78,
		Next = 34,
		NoName = 252,
		None = 0,
		NumLock = 144,
		NumPad0 = 96,
		NumPad1 = 97,
		NumPad2 = 98,
		NumPad3 = 99,
		NumPad4 = 100,
		NumPad5 = 101,
		NumPad6 = 102,
		NumPad7 = 103,
		NumPad8 = 104,
		NumPad9 = 105,
		O = 79,
		Oem8 = 223,
		OemBackslash = 226,
		OemClear = 254,
		OemCloseBrackets = 221,
		Oemcomma = 188,
		OemMinus = 189,
		OemOpenBrackets = 219,
		OemPeriod = 190,
		OemPipe = 220,
		Oemplus = 187,
		OemQuestion = 191,
		OemQuotes = 222,
		OemSemicolon = 186,
		Oemtilde = 192,
		P = 80,
		Pa1 = 253,
		PageDown = 34,
		PageUp = 33,
		Pause = 19,
		Play = 250,
		Print = 42,
		PrintScreen = 44,
		Prior = 33,
		ProcessKey = 229,
		Q = 81,
		R = 82,
		RButton = 2,
		RControlKey = 163,
		Return = 13,
		Right = 39,
		RMenu = 165,
		RShiftKey = 161,
		RWin = 92,
		S = 83,
		Scroll = 145,
		Select = 41,
		SelectMedia = 181,
		Separator = 108,
		Shift = 65536,
		ShiftKey = 16,
		Snapshot = 44,
		Space = 32,
		Subtract = 109,
		T = 84,
		Tab = 9,
		U = 85,
		Up = 38,
		V = 86,
		VolumeDown = 174,
		VolumeMute = 173,
		VolumeUp = 175,
		W = 87,
		X = 88,
		XButton1 = 5,
		XButton2 = 6,
		Y = 89,
		Z = 90,
		Zoom = 251
#pragma warning restore 1591
	}
}
