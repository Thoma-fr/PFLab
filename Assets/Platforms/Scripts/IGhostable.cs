using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGhostable
{
    void Ghostify();

    /// <summary> Un-ghostify </summary>
    void RenderPhysical();
}
