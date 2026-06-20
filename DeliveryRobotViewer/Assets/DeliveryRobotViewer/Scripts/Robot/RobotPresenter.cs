using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// æ∆¿Ã≈∆ 1∞≥¥Á 1
/// </summary>
public class RobotPresenter
{
    private readonly RobotPresenterFactory _factory;
    private readonly string _id;
    private readonly RobotModel _model;
    private readonly RobotListItem _listItem;
    private readonly RobotMoveItem _moveitem;

    private bool _isSelect;
    int _selectedIndex = 20;
    int _nonSelectedIndex = 0;

    public RobotPresenter(RobotListItem listItem, RobotMoveItem moveItem, RobotPresenterFactory factory, RobotModel model)
    {
        _model = model;
        _model.OnChange += UPdateLiveness;
        _listItem = listItem;
        _moveitem = moveItem;
        _factory = factory;
        _listItem.OnHoverEnter += MoveEnter;
        _listItem.OnHoverExit += MoveExit;
        _listItem.OnClickde += OnClick;
        _moveitem.OnHoverEnter += MoveEnter;
        _moveitem.OnHoverExit += MoveExit;
        _moveitem.OnClickde += OnClick;
    }


    private void MoveEnter()
    {
        _moveitem.mark.SetActive(true);
        _listItem.bg.color = _listItem.enter;
    }
    private void MoveExit()
    {
        if (_isSelect) return;
        _listItem.bg.color = _listItem.exit;
        _moveitem.mark.SetActive(false);
    }

    public void Select()
    {
        _isSelect = true;
       // _moveitem.cam.Priority = _selectedIndex;

    }
    public void Deselect()
    {
        _isSelect = false;
       // _moveitem.cam.Priority = _nonSelectedIndex;
        _moveitem.mark.SetActive(false);
        _listItem.bg.color = _listItem.exit;
    }

    public void OnClick()
    {
        _factory.SelcetPresent(this);
    }

    public void AllUPdate(RobotModel model)
    {

        _listItem.OnListUpData(model);
        _moveitem.OnMoveUpData(model);
    }
    public void UPdateLiveness(RobotModel model)
    {
        if (_moveitem == null) return;
        if (_factory._currentPresenter == this) { _factory._currentPresenter = null; }
        Reset();
        _factory._robotListViewsPool.Release(_listItem);
        _factory._robotViewsPool.Release(_moveitem);
        _factory.robots.Remove(model.robotId);

    }

    void Reset()
    {
        _isSelect = false;
       // _moveitem.cam.Priority = _nonSelectedIndex;
        _moveitem.mark.SetActive(false);
        _listItem.bg.color = _listItem.exit;
        _listItem.OnHoverEnter -= MoveEnter;
        _listItem.OnHoverExit -= MoveExit;
        _listItem.OnClickde -= OnClick;
        _moveitem.OnHoverEnter -= MoveEnter;
        _moveitem.OnHoverExit -= MoveExit;
        _moveitem.OnClickde -= OnClick;
        _model.OnChange -= UPdateLiveness;
    }
}

