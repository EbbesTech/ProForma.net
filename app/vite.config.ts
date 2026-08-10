import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

//

const host = "localhost";

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  resolve: {
    tsconfigPaths: true,
  },
  clearScreen: true,
  build: {
    sourcemap: true,
    outDir: '../app-shell/ProForma/wwwroot',
  },
  server: {
    port: 3000,
    strictPort: true,
    host: host || false,
    hmr: {
      protocol: "ws",
      host: host || "localhost",
      port: 1421,
    },
    watch: {
      // 3. tell Vite to ignore watching `src-tauri`
      ignored: [""],
    },
  },
})
