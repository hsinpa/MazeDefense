using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TD.Map;

public class GameInteractorCtrl : MonoBehaviour
{
    GameInputManager _gameInputManager;
    TileNode currentSelectedNode;

    [SerializeField]
    CanvasGroup ConstructionUICanvas;

    public void SetUp(GameInputManager gameInputManager) {
        _gameInputManager = gameInputManager;
        _gameInputManager.OnSelectTileNode += SelectTileListener;
    }

    private void SelectTileListener(TileNode tileNode) {
        currentSelectedNode = tileNode;
        Debug.Log(currentSelectedNode.WorldSpace);

        ConstructionUICanvas.transform.position = currentSelectedNode.WorldSpace;
        EnableConstructionUI(true);
    }

    public void EnableConstructionUI(bool isOn) {
        ConstructionUICanvas.alpha = (isOn) ? 1 : 0;
        ConstructionUICanvas.blocksRaycasts = isOn;
        ConstructionUICanvas.interactable = isOn;
    }

    private void OnDestroy()
    {
        if (_gameInputManager != null)
            _gameInputManager.OnSelectTileNode -= SelectTileListener;
    }
}
