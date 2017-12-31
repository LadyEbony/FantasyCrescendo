using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

public class TestInputSource : IInputSource<GameInput> {

  GameInput input;

  public TestInputSource(GameConfig config) {
    input = new GameInput(config);
    for (int i = 0; i < input.PlayerInputs.Length; i++) {
      input.PlayerInputs[i].IsValid = true;
    }
  }

  public GameInput SampleInput() {
    var playerInput = new PlayerInput {
      Movement = new Vector2(ButtonAxis(KeyCode.A, KeyCode.D), ButtonAxis(KeyCode.S, KeyCode.W)),
      //TODO(james7132): Make Tap Jump Configurable
      Jump = Input.GetKey(KeyCode.W),
      IsValid = true
    };
    var inputValue = input.Clone();
    for (int i = 0; i < inputValue.PlayerInputs.Length; i++) {
      if (i == 0)
        inputValue.PlayerInputs[i] = playerInput;
    }
    return inputValue;
  }

  float ButtonAxis(KeyCode neg, KeyCode pos) {
    var val = Input.GetKey(neg) ? -1.0f : 0.0f;
    return val + (Input.GetKey(pos) ? 1.0f : 0.0f);
  }

}

}
