import express, { Request, Response, RequestHandler } from "express";
import cors from "cors";

const app = express();
const PORT = 6969;

app.use(cors());
app.use(express.json());

// Interface for poster
interface Post {
	id: number;
	author: string;
	content: string;
}

// Eksempler på poster
let posts: Post[] = [
	{ id: 1, author: "Seigmann", content: "Au! Noen spiste beina mine!" },
	{ id: 2, author: "ikke Henrik", content: "Jeg har ikke spist noen bein..." },
];

// Henter alle poster
app.get("/api/posts", ((req: Request, res: Response) => {
	res.json(posts);
}) as RequestHandler);

// Legger til en ny post
app.post("/api/posts", ((req: Request, res: Response) => {
	const { author, content } = req.body;

	if (!author || !content) {
		return res.status(400).json({ error: "Navn og innhold er påkrevd på server." });
	}

	const newPost: Post = {
		id: posts.length + 1,
		author,
		content,
	};

	posts.push(newPost);
	res.status(201).json(newPost);
}) as RequestHandler);

// Sletter en post
app.delete("/api/posts/:id", ((req: Request, res: Response) => {
	const { id } = req.params;
	const index = posts.findIndex((post) => post.id === parseInt(id));

	if (index === -1) {
		return res.status(404).json({ error: "Posten finnes ikke." });
	}

	posts.splice(index, 1);
	res.status(204).send();
}) as RequestHandler);

// Hører på porten vår
app.listen(PORT, () => {
	console.log(`Server running on http://localhost:${PORT}`);
});
