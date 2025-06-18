<script lang="ts">
	import { onMount } from "svelte";
	import { fly, scale } from "svelte/transition";
	import { cubicOut } from "svelte/easing";

	interface Post {
		id: number;
		author: string;
		content: string;
	}

	let posts: Post[] = [];
	let author = "";
	let content = "";
	let error = "";

	// Henter alle poster fra backend
	async function fetchPosts() {
		const res = await fetch("http://localhost:6969/api/posts");
		posts = await res.json();
	}

	// Legger til en ny post i backend
	async function addPost() {
		error = "";

		// Vi kan også validere på klienten
		// if (!author || !content) {
		// 	error = "Navn og innhold er påkrevd før vi kjører kallet!";
		// 	return;
		// }

		const res = await fetch("http://localhost:6969/api/posts", {
			method: "POST",
			headers: { "Content-Type": "application/json" },
			body: JSON.stringify({ author, content }),
		});

		if (res.ok) {
			const newPost = await res.json();
			posts = [...posts, newPost];
			author = "";
			content = "";
		} else {
			const err = await res.json();
			error = err.error || "Kunne ikke legge til post.";
		}
	}

	// Sletter en post fra backend
	async function deletePost(id: number) {
		const res = await fetch(`http://localhost:6969/api/posts/${id}`, { method: "DELETE" });
		if (res.ok) {
			posts = posts.filter((post) => post.id !== id);
		}
	}

	onMount(fetchPosts);
</script>

<main>
	<h1>Fullstack forum Node.js demo</h1>

	<div class="form-container">
		<form on:submit|preventDefault={addPost}>
			<div class="animated-gradient-input">
				<input placeholder="Navn" bind:value={author} />
			</div>
			<div class="animated-gradient-input">
				<input placeholder="Innhold" bind:value={content} />
			</div>
			<button type="submit">Legg til</button>
		</form>
		{#if error}
			<p class="error">{error}</p>
		{/if}
	</div>

	<h2>Alle poster</h2>
	<ul>
		{#each posts as post (post.id)}
			<li in:fly={{ y: 40, opacity: 0, duration: 500, easing: cubicOut }}>
				<div>
					<strong>{post.author}:</strong>
					{post.content}
				</div>
				<button class="delete-button" on:click={() => deletePost(post.id)}>x</button>
			</li>
		{/each}
	</ul>
</main>

<footer>
	<p class="footer-text">Server-side ruler!</p>
</footer>

<style>
	:root {
		--primary-color: #7300ff;
		--secondary-color: #5e00c9;
		--background-color: #11102b;
		--text-color: #fff;
		--error-color: #ff0000;
	}

	:global(html),
	:global(body) {
		background-color: var(--background-color);
		color: var(--text-color);
		font-family: "Space Grotesk", "Segoe UI", Arial, sans-serif;
	}
	main {
		max-width: 500px;
		margin: 2rem auto;
		font-family: inherit;
	}
	.form-container {
		background: rgba(34, 32, 64, 0.92);
		border-radius: 1.5rem;
		box-shadow: 0 8px 32px rgba(31, 38, 135, 0.15);
		padding: 2.5rem 2rem 1.5rem 2rem;
		margin-bottom: 2rem;
	}
	form {
		display: flex;
		flex-direction: column;
		gap: 1.2rem;
		margin-bottom: 1rem;
	}
	.animated-gradient-input {
		position: relative;
		display: flex;
		align-items: center;
	}
	.animated-gradient-input input {
		width: 100%;
		padding: 1rem;
		border: none;
		border-radius: 0.75rem;
		background: var(--background-color);
		color: var(--text-color);
		font-size: 1.1rem;
		z-index: 1;
		position: relative;
		box-shadow: 0 2px 8px rgba(115, 0, 255, 0.07);
		transition: box-shadow 0.2s;
	}
	.animated-gradient-input::before {
		content: "";
		position: absolute;
		top: -3px;
		left: -3px;
		right: -3px;
		bottom: -3px;
		z-index: 0;
		border-radius: 1rem;
		padding: 0;
		background: linear-gradient(270deg, var(--primary-color), var(--secondary-color), #ffc800, var(--primary-color));
		background-size: 600% 600%;
		animation: gradient-border 4s ease infinite;
	}
	@keyframes gradient-border {
		0% {
			background-position: 0% 50%;
		}
		50% {
			background-position: 100% 50%;
		}
		100% {
			background-position: 0% 50%;
		}
	}
	.animated-gradient-input input:focus {
		outline: none;
		box-shadow: 0 0 0 3px var(--primary-color);
	}
	.error {
		color: var(--error-color);
		margin-top: 0.5rem;
		font-weight: 600;
		text-align: center;
	}
	button {
		padding: 1rem 2rem;
		background: linear-gradient(90deg, var(--primary-color) 0%, var(--secondary-color) 100%);
		color: var(--text-color);
		border: none;
		border-radius: 0.75rem;
		font-size: 1.1rem;
		font-weight: 700;
		box-shadow: 0 2px 8px rgba(115, 0, 255, 0.1);
		transition:
			background 0.3s,
			transform 0.2s;
		cursor: pointer;
	}
	button:hover {
		background: linear-gradient(90deg, var(--secondary-color) 0%, #ff5e00 100%);
		transform: translateY(-2px) scale(1.03);
	}
	h2 {
		margin-top: 3rem;
		color: var(--primary-color);
		font-size: 1.5rem;
		font-weight: 700;
	}
	ul {
		list-style: none;
		padding: 0;
	}
	li {
		display: flex;
		justify-content: space-between;
		align-items: center;
		margin-bottom: 1rem;
		color: var(--text-color);
		font-size: 1.2rem;
		border-radius: 0.5rem;
		background: linear-gradient(90deg, var(--primary-color) 0%, var(--secondary-color) 100%);
		padding: 1rem;
		text-align: left;
		box-shadow: 0 2px 8px rgba(115, 0, 255, 0.1);
	}
	.delete-button {
		border: none;
		padding: 1rem;
		width: 1rem;
		height: 1rem;
		border-radius: 100%;
		display: flex;
		align-items: center;
		justify-content: center;
		color: var(--text-color);
		font-size: 1rem;
		cursor: pointer;
	}
	.delete-button:hover {
		box-shadow: 0 0 0 3px var(--primary-color);
		background: var(--primary-color);
	}
	.footer-text {
		margin-top: 10rem;
	}
</style>
