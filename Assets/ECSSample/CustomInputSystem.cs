using Hsinpa.ECS.Component;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Hsinpa.ECS.System
{
    public class CustomInputSystem
    {
        private NewControls _inputActions;
        private InputComponent _inputComponent;

        public InputComponent InputDataComponent => _inputComponent;

        public CustomInputSystem() {
            _inputActions = new NewControls();
            _inputActions.Enable();
            _inputActions.InputAction.YAxis.started += OnYAxisPerform;
            _inputActions.InputAction.YAxis.canceled += OnYAxisPerform;

            _inputActions.InputAction.XAxis.started += OnXAxisPerform;
            _inputActions.InputAction.XAxis.canceled += OnXAxisPerform;

            _inputComponent = new InputComponent();
            _inputComponent.Axis = new Unity.Mathematics.float2();
        }

        private void OnYAxisPerform(InputAction.CallbackContext callbackContext) {
            var yaxis = callbackContext.ReadValue<float>();
            _inputComponent.Axis.y = yaxis;
        }

        private void OnXAxisPerform(InputAction.CallbackContext callbackContext)
        {
            var xaxis = callbackContext.ReadValue<float>();
            _inputComponent.Axis.x = xaxis;
        }

        public void Dispose() {
            _inputActions.Disable();
            _inputActions.Dispose();
        }
    }
}
