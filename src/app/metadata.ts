import type { Metadata } from "next";

export const metadata: Metadata = {
  title: {
    default: "MiniRent - Apartment & Property Rental Management System",
    template: "%s | MiniRent",
  },
  description:
    "MiniRent is a full-stack apartment and property rental management system for small real estate offices. Manage properties, rentals, inquiries, and track key metrics with role-based access.",
  keywords: [
    "MiniRent",
    "Apartment Rental System",
    "Property Management",
    "Real Estate Management",
    "Rental Tracking",
    "Lead Management",
    "Dashboard",
    "DAFTech Evaluation Project"
  ],
  authors: [{ name: "DAFTech Social Solution Evaluation Project" }],
  creator: "MiniRent Team",
  applicationName: "MiniRent",
  category: "Real Estate Management",
  icons: {
    icon: "/favicon.ico",
  },
  openGraph: {
    title: "MiniRent - Rental Management System",
    description:
      "Manage properties, rental records, and inquiries efficiently with MiniRent.",
    type: "website",
  },
};
