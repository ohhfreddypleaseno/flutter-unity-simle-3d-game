import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:flutter_joystick/flutter_joystick.dart';
import 'package:flutter_unity_widget/flutter_unity_widget.dart';
import 'package:flutter_test/flutter_test.dart';

void main() => runApp(MyApp());

class MyApp extends StatefulWidget {
  @override
  _MyAppState createState() => _MyAppState();
}

class _MyAppState extends State<MyApp> {
  static final GlobalKey<ScaffoldState> _scaffoldKey = GlobalKey<ScaffoldState>();
  late UnityWidgetController _unityWidgetController;
  double _sliderValue = 0.0;

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      home: Scaffold(
        key: _scaffoldKey,
        appBar: AppBar(
          title: const Text('Unity Flutter Demo'),
        ),
        body: Card(
          margin: const EdgeInsets.all(8),
          clipBehavior: Clip.antiAlias,
          shape: RoundedRectangleBorder(
            borderRadius: BorderRadius.circular(20.0),
          ),
          child: Stack(
            children: <Widget>[
              GestureDetector(
                onPanEnd: (_) => stopLook(),
                onPanUpdate: look,
                onPanCancel: stopLook,
                child: UnityWidget(
                  onUnityCreated: onUnityCreated,
                  onUnityMessage: onUnityMessage,
                  onUnitySceneLoaded: onUnitySceneLoaded,
                  fullscreen: false,
                ),
              ),
              Positioned(
                bottom: 20,
                right: 20,
                child: Joystick(
                  listener: move,
                  onStickDragEnd: stop,
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }

  void look(DragUpdateDetails details) {
    print(details.delta);
    _unityWidgetController.postMessage(
      'Player',
      'ChangeDirectionOfView',
      '${details.delta.dx * -1} ${details.delta.dy}',
    );
  }

  void stopLook() {
    _unityWidgetController.postMessage(
      'Player',
      'ChangeDirectionOfView',
      '0 0',
    );
  }

  void stop() {
    _unityWidgetController.postMessage(
      'Player',
      'ChangeDirectionOfMove',
      '0 0',
    );
  }

  void move(StickDragDetails details) {
    _unityWidgetController.postMessage(
      'Player',
      'ChangeDirectionOfMove',
      '${details.x} ${details.y * -1}',
    );
  }

  void moveForward(bool pressed) {
    if (pressed) {

    } else {

    }
  }

  void moveBack(bool pressed) {
    if (pressed) {
      _unityWidgetController.postMessage(
        'Player',
        'ChangeDirectionOfMove',
        '0 -1',
      );
    } else {
      _unityWidgetController.postMessage(
        'Player',
        'ChangeDirectionOfMove',
        '0 0',
      );
    }
  }

  void moveRight(bool pressed) {
    if (pressed) {
      _unityWidgetController.postMessage(
        'Player',
        'ChangeDirectionOfMove',
        '1 0',
      );
    } else {
      _unityWidgetController.postMessage(
        'Player',
        'ChangeDirectionOfMove',
        '0 0',
      );
    }
  }

  void moveLeft(bool pressed) {
    if (pressed) {
      _unityWidgetController.postMessage(
        'Player',
        'ChangeDirectionOfMove',
        '-1 0',
      );
    } else {
      _unityWidgetController.postMessage(
        'Player',
        'ChangeDirectionOfMove',
        '0 0',
      );
    }
  }

  // Communication from Flutter to Unity
  void setRotationSpeed(String speed) {
    _unityWidgetController.postMessage(
      'Cube',
      'SetRotationSpeed',
      speed,
    );
  }

  // Communication from Unity to Flutter
  void onUnityMessage(message) {
    print('Received message from unity: ${message.toString()}');
  }

  // Callback that connects the created controller to the unity controller
  void onUnityCreated(controller) {
    _unityWidgetController = controller;
  }

  // Communication from Unity when new scene is loaded to Flutter
  void onUnitySceneLoaded(SceneLoaded? sceneInfo) {
    print('Received scene loaded from unity: ${sceneInfo?.name}');
    print('Received scene loaded from unity buildIndex: ${sceneInfo?.buildIndex}');
  }
}
