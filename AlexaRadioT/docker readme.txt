# https://www.digitalocean.com/community/tutorials/how-to-install-nginx-on-ubuntu-16-04
# Configure NGINX /etc/nginx/sites-available/default
____________________________
server {
	server_name radio-t.lalala.space;
	location / {
		proxy_set_header HOST $host;
		proxy_set_header X-Forwarded-Proto $scheme;
		proxy_set_header X-Real-IP $remote_addr;
		proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
		proxy_pass http://localhost:8080;
	}
}
server {
	server_name lalala.space www.lalala.space;
	location / {
		proxy_set_header HOST $host;
		proxy_set_header X-Forwarded-Proto $scheme;
		proxy_set_header X-Real-IP $remote_addr;
		proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
		proxy_pass http://localhost:8081;
	}
}
____________________________

# sudo nginx -t
# sudo systemctl reload nginx

# https://www.digitalocean.com/community/tutorials/how-to-secure-nginx-with-let-s-encrypt-on-ubuntu-16-04
sudo certbot --nginx -d radio-t.lalala.space
sudo certbot --nginx -d lalala.space -d www.lalala.space
sudo certbot renew --dry-run

____________________________

# https://www.digitalocean.com/community/tutorials/how-to-install-and-use-docker-on-ubuntu-16-04
docker run -d -e WebApplicationUrl='https://radio-t.lalala.space' --name alexaradiot --restart=unless-stopped -p 8080:80 xtonyx/alexaradiot:20180605065851

____________________________

# WORDPRESS AND MYSQL

docker network create --driver bridge isolated_network

# docker pull mysql:5.7
# https://severalnines.com/blog/mysql-docker-containers-understanding-basics
docker run -d --net=isolated_network --restart=unless-stopped -p 3306:3306 -v /var/docker_mysql:/var/lib/mysql -e MYSQL_ROOT_PASSWORD=pass@word1 --name mysql mysql:5.7
docker inspect mysql
apt-get install mysql-client
mysql -uroot -ppass@word1 -h 172.18.0.2 -P 3306

# https://www.a2hosting.com/kb/developer-corner/mysql/managing-mysql-databases-and-users-from-the-command-line
GRANT ALL PRIVILEGES ON *.* TO 'wp_lalala_space'@'%' IDENTIFIED BY 'pass@word1';
CREATE DATABASE wp_lalala_space;
\q

# docker stop mysql
# docker start mysql
# docker rm mysql
# rm -r /var/docker_mysql

# https://hub.docker.com/r/bitnami/wordpress/
# docker pull bitnami/wordpress:latest
docker run -d --net=isolated_network --restart=unless-stopped -p 8081:80 -v /var/www/docker_wp_lalala_space:/bitnami -e MARIADB_HOST=mysql -e WORDPRESS_DATABASE_NAME=wp_lalala_space -e WORDPRESS_DATABASE_USER=wp_lalala_space -e WORDPRESS_DATABASE_PASSWORD=pass@word1 -e WORDPRESS_USERNAME=anton.ozolin -e WORDPRESS_PASSWORD=pass@word1 -e WORDPRESS_EMAIL=anton.ozolin@gmail.com  --name wp_lalala_space bitnami/wordpress:latest
# docker stop wp_lalala_space
# docker start wp_lalala_space
# docker rm wp_lalala_space
# rm -r /var/www/docker_wp_lalala_space
# https://docs.bitnami.com/general/apps/wordpress/#how-to-change-the-wordpress-domain-name