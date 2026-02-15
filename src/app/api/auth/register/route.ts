import { NextResponse } from "next/server";

export async function POST(request: Request) {
  const token = request.headers.get("authorization") || "";
  const body = await request.json();

  const res = await fetch("http://127.0.0.1:5000/api/auth/register", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      ...(token ? { Authorization: token } : {}),
    },
    body: JSON.stringify(body),
  });

  let data: any = null;
  try {
    data = await res.json();
  } catch {
    data = null;
  }

  if (!res.ok) {
    return NextResponse.json(
      { error: data?.message || "Failed to register user" },
      { status: res.status }
    );
  }

  return NextResponse.json(data);
}

