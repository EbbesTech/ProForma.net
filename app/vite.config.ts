import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  resolve: {
    tsconfigPaths: true,
  },
  clearScreen: true,
  build: {
    outDir: '../app-shell/ProForma/wwwroot',
  }
})
