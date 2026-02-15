import { NextResponse } from "next/server";

export async function POST(
    request: Request,
    { params }: { params: Promise<{ id: string }> }
) {
    const { id } = await params;
    const token = request.headers.get("authorization") || "";

    try {
        const body = await request.json();

        const res = await fetch(`http://127.0.0.1:5000/api/rentals/${id}/end`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                ...(token ? { "Authorization": token } : {})
            },
            body: JSON.stringify(body)
        });

        if (!res.ok) {
            const error = await res.text();
            return NextResponse.json({ error }, { status: res.status });
        }

        const data = await res.json();
        return NextResponse.json(data);
    } catch (error) {
        console.error("End rental error:", error);
        return NextResponse.json(
            { error: "Failed to end rental" },
            { status: 500 }
        );
    }
}
