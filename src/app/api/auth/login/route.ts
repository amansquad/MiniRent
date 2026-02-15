import { NextResponse } from "next/server";

export async function POST(request: Request) {
  try {
    const body = await request.json();

    const res = await fetch("http://127.0.0.1:5000/api/auth/login", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(body),
    });

    if (!res.ok) {
      const errorBody = await res.json().catch(() => null);
      const message =
        (errorBody && (errorBody.message || errorBody.error)) ||
        "Invalid username or password";

      return NextResponse.json(
        { message },
        {
          status: res.status,
        }
      );
    }

    const data = await res.json();

    return NextResponse.json(data);
  } catch (error) {
    console.error("Login error:", error);
    return NextResponse.json(
      { message: "Unable to login. Please try again." },
      { status: 500 }
    );
  }
}

