import type { NextConfig } from "next";

const nextConfig: NextConfig = {
  output: "standalone",
  // Ensure Next.js treats this folder as the project root,
  // even if there are other lockfiles higher up (fixes wrong-root warning).
  experimental: {
    turbo: {
      root: __dirname,
    },
  },
  typescript: {
    ignoreBuildErrors: true,
  },
  reactStrictMode: false,
};

export default nextConfig;
