import react from '@vitejs/plugin-react'
import { defineConfig } from 'vitest/config'
import { loadEnv } from 'vite'

export default defineConfig(({ mode }) => {
  const environment = loadEnv(mode, process.cwd(), '')
  const backendTarget = environment.FARMHELM_BACKEND_PROXY_TARGET?.trim() || 'http://localhost:5222'

  return {
    envPrefix: ['VITE_', 'TAURI_'],
    plugins: [react()],
    server: {
      port: 5173,
      strictPort: true,
      clearScreen: false,
      watch: { ignored: ['**/src-tauri/**'] },
      proxy: {
        '/api': { target: backendTarget, changeOrigin: true },
        '/health': { target: backendTarget, changeOrigin: true },
      },
    },
    test: {
      environment: 'jsdom',
      setupFiles: './src/test/setup.ts',
      globals: true,
    },
  }
})
