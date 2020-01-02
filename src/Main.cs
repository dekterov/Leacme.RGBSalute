// Copyright (c) 2017 Leacme (http://leac.me). View LICENSE.md for more information.
using Godot;
using System;

public class Main : Spatial {

	public AudioStreamPlayer Audio { get; } = new AudioStreamPlayer();

	private void InitSound() {
		if (!Lib.Node.SoundEnabled) {
			AudioServer.SetBusMute(AudioServer.GetBusIndex("Master"), true);
		}
	}

	public override void _Notification(int what) {
		if (what is MainLoop.NotificationWmGoBackRequest) {
			GetTree().ChangeScene("res://scenes/Menu.tscn");
		}
	}

	public override void _Ready() {
		var env = GetNode<WorldEnvironment>("sky").Environment;
		env.BackgroundColor = new Color(Lib.Node.BackgroundColorHtmlCode);
		env.BackgroundMode = Godot.Environment.BGMode.Sky;
		env.BackgroundSky = new PanoramaSky() { Panorama = ((Texture)GD.Load("res://assets/city_night.hdr")) };
		env.BackgroundSkyRotationDegrees = new Vector3(25, 135, 25);
		env.BackgroundEnergy = 0.2f;
		env.GlowEnabled = true;
		env.GlowBicubicUpscale = true;
		env.GlowIntensity = 1;

		InitSound();
		AddChild(Audio);

		var firework = new Particles() {
			OneShot = true,
			Amount = 500,
			SpeedScale = 7,
			Lifetime = 30,
			Explosiveness = 1,
			Randomness = 1,
			DrawPass1 = new QuadMesh(),
			ProcessMaterial = new ParticlesMaterial() {
				FlagAlignY = true,
				FlagRotateY = true,
				Spread = 180,
				Gravity = new Vector3(0, 0, 0),
				InitialVelocity = 0.5f,
				AngularVelocity = 10,
				AngularVelocityRandom = 0.5f,
				ScaleCurve = new CurveTexture() {
					Curve = new Curve() {
						_Data = new Godot.Collections.Array() { new Vector2(0, 0), 0.0, 2.3, 0, 0, new Vector2(0.5f, 0.33f), 0.0, 0.0, 0, 0, new Vector2(1, 0), -2.3, 0.0, 0, 0, },
					}
				}

			}
		};

		var fwTimer = new System.Timers.Timer() { AutoReset = true, Enabled = true };
		fwTimer.Elapsed += (z, zz) => {
			fwTimer.Interval = GD.RandRange(1000, 1500);
			var tempFw = (Particles)firework.Duplicate();
			var randColor = Color.FromHsv(GD.Randf(), 1, 1);

			tempFw.DrawPass1 = new QuadMesh() {
				Material = new SpatialMaterial() {
					AlbedoColor = randColor,
					EmissionEnabled = true,
					Emission = randColor,
					EmissionEnergy = 10
				}
			};
			AddChild(tempFw);
			tempFw.Translation = new Vector3(0, (float)GD.RandRange(-10, 30), (float)GD.RandRange(-15, 15));
		};

	}

	public override void _Process(float delta) {

	}

}
