import { NextResponse } from "next/server";

type ParamsPromise = { params: Promise<{ id: string }> };

export async function PUT(request: Request, context: ParamsPromise) {
  const { id } = await context.params;

  const token = request.headers.get("authorization") || "";
  const body = await request.json();

  const res = await fetch(`http://127.0.0.1:5000/api/users/${id}`, {
    method: "PUT",
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
      { error: data?.message || "Failed to update user" },
      { status: res.status }
    );
  }

  return NextResponse.json(data);
}

export async function DELETE(request: Request, context: ParamsPromise) {
  const { id } = await context.params;

  const token = request.headers.get("authorization") || "";

  const res = await fetch(`http://127.0.0.1:5000/api/users/${id}`, {
    method: "DELETE",
    headers: {
      "Content-Type": "application/json",
      ...(token ? { Authorization: token } : {}),
    },
  });

  if (!res.ok) {
    let data: any = null;
    try {
      data = await res.json();
    } catch {
      data = null;
    }
    return NextResponse.json(
      { error: data?.message || "Failed to delete user" },
      { status: res.status }
    );
  }

  return NextResponse.json({ success: true });
}
