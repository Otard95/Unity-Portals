﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalTextureSetup : MonoBehaviour {

	public RenderTexture[] portalTextures;

	void Start () {
		
		foreach (var tex in portalTextures) {
			tex.height = Screen.height;
			tex.width = Screen.width;
		}

	}

}
