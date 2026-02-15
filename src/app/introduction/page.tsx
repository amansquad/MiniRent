"use client";

export default function IntroductionPage() {
  return (
    <div className="p-8 space-y-4">
      <h1 className="text-2xl font-bold">Introduction</h1>
      <p className="text-muted-foreground">
        MiniRent is a lightweight property rental management dashboard. It helps small
        landlords and agencies track properties, rentals, inquiries, and users in one
        simple interface.
      </p>
      <p className="text-muted-foreground">
        Use the sidebar to navigate between sections. After logging in as an admin, you
        can manage your portfolio, onboard new users, and get a quick overview of your
        business from the dashboard.
      </p>
    </div>
  );
}

