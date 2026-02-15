"use client";

export default function AboutPage() {
  return (
    <div className="p-8 space-y-4">
      <h1 className="text-2xl font-bold">About MiniRent</h1>
      <p className="text-muted-foreground">
        MiniRent was built as a focused tool for managing small to medium rental
        portfolios. Instead of a bloated system with dozens of modules, it gives you just
        what you need: properties, rentals, inquiries, users, and a clean dashboard.
      </p>
      <p className="text-muted-foreground">
        The backend is powered by ASP.NET Core and PostgreSQL, while the frontend uses
        Next.js, Tailwind CSS, and a modern component library for a fast and responsive
        experience.
      </p>
      <p className="text-muted-foreground">
        This project is intended as a starting point — you can extend it with your own
        workflows, reports, and integrations as your needs grow.
      </p>
    </div>
  );
}

