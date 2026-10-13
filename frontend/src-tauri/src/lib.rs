use std::{net::TcpStream, sync::Mutex, thread, time::Duration};
use tauri::{Manager, RunEvent};
use tauri_plugin_shell::{ShellExt, process::CommandChild};

struct Sidecar(Mutex<Option<CommandChild>>);

fn wait_for_sidecar() -> bool {
    for _ in 0..50 {
        if TcpStream::connect("127.0.0.1:5223").is_ok() {
            return true;
        }
        thread::sleep(Duration::from_millis(200));
    }
    false
}

fn stop_sidecar(app: &tauri::AppHandle) {
    if let Some(state) = app.try_state::<Sidecar>() {
        if let Some(child) = state.0.lock().expect("sidecar state lock").take() {
            let _ = child.kill();
        }
    }
}

pub fn run() {
    tauri::Builder::default()
        .plugin(tauri_plugin_shell::init())
        .setup(|app| {
            let (_, child) = app
                .shell()
                .sidecar("farmhelm-api")?
                .args(["--urls", "http://127.0.0.1:5223"])
                .spawn()?;
            app.manage(Sidecar(Mutex::new(Some(child))));
            if !wait_for_sidecar() {
                stop_sidecar(&app.handle());
                return Err("FarmHelm API sidecar did not become ready on 127.0.0.1:5223".into());
            }
            Ok(())
        })
        .build(tauri::generate_context!())
        .expect("error while building FarmHelm desktop")
        .run(|app, event| {
            if matches!(event, RunEvent::Exit | RunEvent::ExitRequested { .. }) {
                stop_sidecar(app);
            }
        });
}
